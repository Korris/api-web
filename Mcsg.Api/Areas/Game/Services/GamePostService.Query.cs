using Microsoft.EntityFrameworkCore;

namespace Mcsg.Api.Areas.Game.Services;

using Common.Core.Enums;
using Common.Domain.Entities;
using Common.SeedWork.Exceptions;
using Common.SeedWork.Responses;
using Mcsg.Api.Areas.Game.Models;
using Mcsg.Api.Areas.Game.Requests;
using static Common.SeedWork.Constants.Error;

/// <summary>
/// Game post read queries (detail / list / my list) with Permission enforcement, comment count and reactions
/// </summary>
public partial class GamePostService
{
    #region -- Methods --

    public Task<GamePostResponse> GetAsync(GameHashIdR request)
    {
        return GetByHashIdAsync(request.HashId, request.UserId, request.IsPremium);
    }

    public Task<PagedResponse<GamePostResponse>> ListAsync(GamePostListR request)
    {
        // Public feed: only Public status, never Private permission
        var query = BaseQuery()
            .Where(p => p.Status == PostStatus.Public && p.Permission != PostPermission.Private);

        if (!string.IsNullOrWhiteSpace(request.ProfileName))
        {
            query = query.Where(p => p.User.ProfileName == request.ProfileName);
        }
        if (request.FromMobile)
        {
            query = query.Where(p => p.IsMature != true);
        }
        return PageAsync(query, request);
    }

    public Task<PagedResponse<GamePostResponse>> ListMineAsync(GamePostListR request)
    {
        if (request.UserId == null)
        {
            throw new BadRequestException(nameof(E109), E109);
        }
        var query = BaseQuery().Where(p => p.UserId == request.UserId);
        return PageAsync(query, request);
    }

    #endregion

    #region -- Helpers --

    /// <summary>
    /// Detail by hash id. Non-public status or Private permission → only the owner (404 otherwise, no existence leak).
    /// Premium → GameUrl hidden unless the viewer is premium or the owner.
    /// </summary>
    private async Task<GamePostResponse> GetByHashIdAsync(string? hashId, Guid? currentUserId, bool isPremiumViewer)
    {
        var post = await Project(BaseQuery(), currentUserId)
            .FirstOrDefaultAsync(p => p.HashId == hashId);

        var isOwner = post != null && currentUserId != null && post.UserId == currentUserId;
        if (post == null || (!isOwner && (post.Status != PostStatus.Public || post.Permission == PostPermission.Private)))
        {
            throw new NotFoundException(nameof(E204), E204);
        }
        if (!isOwner && post.Permission == PostPermission.Premium && !isPremiumViewer)
        {
            post.GameUrl = null;
        }
        await AttachReactionsAsync(new List<GamePostResponse> { post }, currentUserId);
        return post;
    }

    private IQueryable<GamePost> BaseQuery()
    {
        return _context.Available<GamePost>(false);
    }

    private async Task<PagedResponse<GamePostResponse>> PageAsync(IQueryable<GamePost> query, GamePostListR request)
    {
        if (!string.IsNullOrWhiteSpace(request.Keyword))
        {
            var keyword = request.Keyword.Trim();
            query = query.Where(p => p.Title != null && EF.Functions.ILike(p.Title, $"%{keyword}%"));
        }

        var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
        var pageSize = request.PageSize < 1 ? DefaultPageSize : Math.Min(request.PageSize, MaxPageSize);
        var total = await query.CountAsync();

        var items = await Project(query.OrderByDescending(p => p.CreatedOn), request.UserId)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        await AttachReactionsAsync(items, request.UserId);

        // List never exposes premium game files to non-premium viewers
        if (!request.IsPremium)
        {
            foreach (var item in items.Where(p => p.Permission == PostPermission.Premium && !p.IsCurrentUserAuthor))
            {
                item.GameUrl = null;
            }
        }

        return new PagedResponse<GamePostResponse>(items, total, pageNumber, pageSize);
    }

    /// <summary>
    /// One grouped reaction query for the whole page
    /// </summary>
    private async Task AttachReactionsAsync(List<GamePostResponse> items, Guid? currentUserId)
    {
        var summaries = await _reactService.GetPostSummariesAsync(items.Select(p => p.Id), currentUserId);
        foreach (var item in items)
        {
            item.Reaction = summaries.TryGetValue(item.Id, out var summary) ? summary : new ReactionSummaryResponse { TargetId = item.Id };
        }
    }

    /// <summary>
    /// Entity → response projection (single place so detail and list return the same shape)
    /// </summary>
    private static IQueryable<GamePostResponse> Project(IQueryable<GamePost> query, Guid? currentUserId)
    {
        return query.Select(p => new GamePostResponse
        {
            Id = p.Id,
            HashId = p.HashId!,
            Title = p.Title,
            Body = p.Body,
            ThumbnailUrl = p.ThumbnailUrl,
            GameUrl = p.GameUrl,
            AuthorName = p.AuthorName,
            IsCurrentUserAuthor = currentUserId != null && p.UserId == currentUserId,
            IsMature = p.IsMature == true,
            Permission = p.Permission,
            Status = p.Status,
            Type = p.Type,
            ViewCount = p.ViewCount,
            CreatedOn = p.CreatedOn,
            ModifiedOn = p.ModifiedOn,
            UserId = p.UserId,
            UserName = p.User.UserName,
            ProfileName = p.User.ProfileName,
            ProfileId = p.User.ProfileId,
            UserAvatar = p.User.Avatar,
            CommentCount = p.GamePostComments.Count(c => !c.IsDelete && c.Status == CommentStatus.Public)
        });
    }

    #endregion

    #region -- Fields --

    private const int DefaultPageSize = 10;
    private const int MaxPageSize = 50;

    #endregion
}
