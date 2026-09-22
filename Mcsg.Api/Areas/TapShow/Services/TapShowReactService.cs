using Microsoft.EntityFrameworkCore;

namespace Mcsg.Api.Areas.TapShow.Services;

using Common.Core.Enums;
using Common.Domain;
using Common.Domain.Entities;
using Common.SeedWork.Exceptions;
using Mcsg.Api.Areas.TapShow.Interfaces;
using Mcsg.Api.Areas.TapShow.Models;
using Mcsg.Api.Areas.TapShow.Requests;
using static Common.SeedWork.Constants.Error;

/// <summary>
/// Reactions for TapShow posts / comments. Copy of GameReactService: one row per user per target, re-react changes the type.
/// </summary>
public class TapShowReactService : ITapShowReactService
{
    #region -- Methods --

    public TapShowReactService(IMcsgContext context)
    {
        _context = context;
    }

    public async Task<ReactionSummaryResponse> ReactToPostAsync(ReactionReactR request)
    {
        // Same visibility rule as post detail: non-public status or Private permission → owner only
        var ok = await _context.Available<TapShowPost>(false)
            .AnyAsync(p => p.Id == request.TargetId
                           && ((p.Status == PostStatus.Public && p.Permission != PostPermission.Private) || p.UserId == request.UserId));
        if (!ok)
        {
            throw new NotFoundException(nameof(E204), E204);
        }
        return await UpsertAsync(_context.TapShowPostReactions, request);
    }

    public Task<ReactionSummaryResponse> RemovePostReactionAsync(ReactionReactR request)
    {
        return RemoveAsync(_context.TapShowPostReactions, request);
    }

    public async Task<ReactionSummaryResponse> ReactToCommentAsync(ReactionReactR request)
    {
        var exists = await _context.Available<TapShowPostComment>(false)
            .AnyAsync(c => c.Id == request.TargetId && c.Status == CommentStatus.Public && !c.Post.IsDelete
                           && ((c.Post.Status == PostStatus.Public && c.Post.Permission != PostPermission.Private) || c.Post.UserId == request.UserId));
        if (!exists)
        {
            throw new NotFoundException(nameof(E204), E204);
        }
        return await UpsertAsync(_context.TapShowPostCommentReactions, request);
    }

    public Task<ReactionSummaryResponse> RemoveCommentReactionAsync(ReactionReactR request)
    {
        return RemoveAsync(_context.TapShowPostCommentReactions, request);
    }

    public Task<Dictionary<Guid, ReactionSummaryResponse>> GetPostSummariesAsync(IEnumerable<Guid> postIds, Guid? userId)
    {
        return SummariesAsync(_context.TapShowPostReactions, postIds, userId);
    }

    public Task<Dictionary<Guid, ReactionSummaryResponse>> GetCommentSummariesAsync(IEnumerable<Guid> commentIds, Guid? userId)
    {
        return SummariesAsync(_context.TapShowPostCommentReactions, commentIds, userId);
    }

    #endregion

    #region -- Helpers --

    private static Guid RequireUser(Guid? userId)
    {
        if (userId == null || userId == Guid.Empty)
        {
            throw new BadRequestException(nameof(E109), E109);
        }
        return userId.Value;
    }

    private async Task<ReactionSummaryResponse> UpsertAsync<T>(DbSet<T> set, ReactionReactR request) where T : BaseReaction, new()
    {
        var userId = RequireUser(request.UserId);
        var existing = await set.FirstOrDefaultAsync(r => r.TargetId == request.TargetId && r.AuthorId == userId && !r.IsDelete);
        if (existing == null)
        {
            await set.AddAsync(new T { TargetId = request.TargetId, AuthorId = userId, Type = request.Type, CreatedBy = userId });
        }
        else if (existing.Type != request.Type)
        {
            existing.Type = request.Type;
            existing.ModifiedBy = userId;
            existing.ModifiedOn = DateTime.UtcNow;
        }
        await _context.SaveChangesAsync(default);
        return await SummaryAsync(set, request.TargetId, userId);
    }

    private async Task<ReactionSummaryResponse> RemoveAsync<T>(DbSet<T> set, ReactionReactR request) where T : BaseReaction
    {
        var userId = RequireUser(request.UserId);
        var existing = await set.FirstOrDefaultAsync(r => r.TargetId == request.TargetId && r.AuthorId == userId && !r.IsDelete);
        if (existing != null)
        {
            existing.IsDelete = true;
            existing.ModifiedBy = userId;
            existing.ModifiedOn = DateTime.UtcNow;
            await _context.SaveChangesAsync(default);
        }
        return await SummaryAsync(set, request.TargetId, userId);
    }

    private static async Task<ReactionSummaryResponse> SummaryAsync<T>(DbSet<T> set, Guid targetId, Guid? userId) where T : BaseReaction
    {
        var map = await SummariesAsync(set, new[] { targetId }, userId);
        return map.TryGetValue(targetId, out var s) ? s : new ReactionSummaryResponse { TargetId = targetId };
    }

    /// <summary>
    /// One grouped query for many targets: counts per type + the current user's own type
    /// </summary>
    private static async Task<Dictionary<Guid, ReactionSummaryResponse>> SummariesAsync<T>(DbSet<T> set, IEnumerable<Guid> targetIds, Guid? userId) where T : BaseReaction
    {
        var ids = targetIds.Distinct().ToList();
        var result = new Dictionary<Guid, ReactionSummaryResponse>();
        if (ids.Count == 0)
        {
            return result;
        }

        var rows = await set.AsNoTracking()
            .Where(r => ids.Contains(r.TargetId) && !r.IsDelete)
            .GroupBy(r => new { r.TargetId, r.Type })
            .Select(g => new { g.Key.TargetId, g.Key.Type, Count = g.Count(), Mine = userId != null && g.Any(r => r.AuthorId == userId) })
            .ToListAsync();

        foreach (var group in rows.GroupBy(r => r.TargetId))
        {
            var reactions = group.OrderByDescending(r => r.Count)
                .Select(r => new ReactionCountResponse { Type = r.Type, Count = r.Count })
                .ToList();
            result[group.Key] = new ReactionSummaryResponse
            {
                TargetId = group.Key,
                TotalReacts = reactions.Sum(r => r.Count),
                Reactions = reactions,
                MostReactionType = reactions.FirstOrDefault()?.Type,
                CurrentUserReactType = group.FirstOrDefault(r => r.Mine)?.Type
            };
        }
        return result;
    }

    #endregion

    #region -- Fields --

    private readonly IMcsgContext _context;

    #endregion
}
