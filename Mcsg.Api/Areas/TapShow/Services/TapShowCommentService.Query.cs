using Microsoft.EntityFrameworkCore;

namespace Mcsg.Api.Areas.TapShow.Services;

using Common.Core.Enums;
using Common.Domain.Entities;
using Common.SeedWork.Exceptions;
using Common.SeedWork.Responses;
using Mcsg.Api.Areas.TapShow.Models;
using Mcsg.Api.Areas.TapShow.Requests;
using static Common.SeedWork.Constants.Error;

/// <summary>
/// Comment read queries: root comments of a post (newest first) or replies of a comment (oldest first)
/// </summary>
public partial class TapShowCommentService
{
    #region -- Methods --

    public async Task<PagedResponse<CommentResponse>> ListAsync(CommentListR request)
    {
        IQueryable<TapShowPostComment> query;
        if (request.ParentId != null)
        {
            // Replies inherit the post visibility rule through the parent's post
            query = BaseQuery()
                .Where(c => c.ParentId == request.ParentId
                            && ((c.Post.Status == PostStatus.Public && c.Post.Permission != PostPermission.Private) || c.Post.UserId == request.UserId))
                .OrderBy(c => c.CreatedOn);
        }
        else
        {
            var postId = await _context.Available<TapShowPost>(false)
                .Where(p => p.HashId == request.PostHashId
                            && ((p.Status == PostStatus.Public && p.Permission != PostPermission.Private) || p.UserId == request.UserId))
                .Select(p => (Guid?)p.Id)
                .FirstOrDefaultAsync();
            if (postId == null)
            {
                throw new NotFoundException(nameof(E204), E204);
            }
            query = BaseQuery().Where(c => c.PostId == postId && c.ParentId == null).OrderByDescending(c => c.CreatedOn);
        }

        var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
        var pageSize = request.PageSize < 1 ? DefaultPageSize : Math.Min(request.PageSize, MaxPageSize);
        var total = await query.CountAsync();
        var items = await Project(query, request.UserId).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        await AttachReactionsAsync(items, request.UserId);

        return new PagedResponse<CommentResponse>(items, total, pageNumber, pageSize);
    }

    /// <summary>
    /// Top root comments of each post for list previews: most reactions first, newest first on ties (like Story list)
    /// </summary>
    public async Task<Dictionary<Guid, List<CommentResponse>>> GetTopByPostIdsAsync(IReadOnlyCollection<Guid> postIds, Guid? currentUserId, int take)
    {
        if (postIds.Count == 0 || take < 1)
        {
            return new Dictionary<Guid, List<CommentResponse>>();
        }

        var ids = await _context.Available<TapShowPost>(false)
            .Where(p => postIds.Contains(p.Id))
            .SelectMany(p => p.TapShowPostComments
                .Where(c => !c.IsDelete && c.ParentId == null && c.Status == CommentStatus.Public)
                .OrderByDescending(c => c.TapShowPostCommentReactions.Count(r => !r.IsDelete))
                .ThenByDescending(c => c.CreatedOn)
                .Take(take)
                .Select(c => c.Id))
            .ToListAsync();

        var items = await Project(BaseQuery().Where(c => ids.Contains(c.Id)), currentUserId).ToListAsync();
        await AttachReactionsAsync(items, currentUserId);

        return items
            .GroupBy(c => c.PostId)
            .ToDictionary(g => g.Key, g => g
                .OrderByDescending(c => c.Reaction?.TotalReacts ?? 0)
                .ThenByDescending(c => c.CreatedOn)
                .ToList());
    }

    #endregion

    #region -- Helpers --

    private async Task<CommentResponse> GetByIdAsync(Guid id, Guid? currentUserId)
    {
        var item = await Project(BaseQuery().Where(c => c.Id == id), currentUserId).FirstOrDefaultAsync()
                   ?? throw new NotFoundException(nameof(E204), E204);
        await AttachReactionsAsync(new List<CommentResponse> { item }, currentUserId);
        return item;
    }

    private IQueryable<TapShowPostComment> BaseQuery()
    {
        return _context.Available<TapShowPostComment>(false).Where(c => c.Status == CommentStatus.Public && !c.Post.IsDelete);
    }

    private async Task AttachReactionsAsync(List<CommentResponse> items, Guid? currentUserId)
    {
        var summaries = await _reactService.GetCommentSummariesAsync(items.Select(c => c.Id), currentUserId);
        foreach (var item in items)
        {
            item.Reaction = summaries.TryGetValue(item.Id, out var s) ? s : new ReactionSummaryResponse { TargetId = item.Id };
        }
    }

    private static IQueryable<CommentResponse> Project(IQueryable<TapShowPostComment> query, Guid? currentUserId)
    {
        return query.Select(c => new CommentResponse
        {
            Id = c.Id,
            PostId = c.PostId,
            PostHashId = c.Post.HashId,
            ParentId = c.ParentId,
            Body = c.Body,
            CreatedOn = c.CreatedOn,
            ModifiedOn = c.ModifiedOn,
            AuthorId = c.AuthorId,
            AuthorName = c.Author.ProfileName,
            UserName = c.Author.UserName,
            ProfileId = c.Author.ProfileId,
            UserAvatar = c.Author.Avatar,
            IsCurrentUserAuthor = currentUserId != null && c.AuthorId == currentUserId,
            ReplyCount = c.ParentId == null
                ? c.Post.TapShowPostComments.Count(r => r.ParentId == c.Id && !r.IsDelete && r.Status == CommentStatus.Public)
                : 0
        });
    }

    #endregion

    #region -- Fields --

    private const int DefaultPageSize = 10;
    private const int MaxPageSize = 50;

    #endregion
}
