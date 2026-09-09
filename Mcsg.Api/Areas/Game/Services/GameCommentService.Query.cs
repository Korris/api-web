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
/// Comment read queries: root comments of a post (newest first) or replies of a comment (oldest first)
/// </summary>
public partial class GameCommentService
{
    #region -- Methods --

    public async Task<PagedResponse<CommentResponse>> ListAsync(CommentListR request)
    {
        IQueryable<GamePostComment> query;
        if (request.ParentId != null)
        {
            query = BaseQuery().Where(c => c.ParentId == request.ParentId).OrderBy(c => c.CreatedOn);
        }
        else
        {
            var postId = await _context.Available<GamePost>(false)
                .Where(p => p.HashId == request.PostHashId && (p.Status == PostStatus.Public || p.UserId == request.UserId))
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

    #endregion

    #region -- Helpers --

    private async Task<CommentResponse> GetByIdAsync(Guid id, Guid? currentUserId)
    {
        var item = await Project(BaseQuery().Where(c => c.Id == id), currentUserId).FirstOrDefaultAsync()
                   ?? throw new NotFoundException(nameof(E204), E204);
        await AttachReactionsAsync(new List<CommentResponse> { item }, currentUserId);
        return item;
    }

    private IQueryable<GamePostComment> BaseQuery()
    {
        return _context.Available<GamePostComment>(false).Where(c => c.Status == CommentStatus.Public && !c.Post.IsDelete);
    }

    private async Task AttachReactionsAsync(List<CommentResponse> items, Guid? currentUserId)
    {
        var summaries = await _reactService.GetCommentSummariesAsync(items.Select(c => c.Id), currentUserId);
        foreach (var item in items)
        {
            item.Reaction = summaries.TryGetValue(item.Id, out var s) ? s : new ReactionSummaryResponse { TargetId = item.Id };
        }
    }

    private static IQueryable<CommentResponse> Project(IQueryable<GamePostComment> query, Guid? currentUserId)
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
                ? c.Post.GamePostComments.Count(r => r.ParentId == c.Id && !r.IsDelete && r.Status == CommentStatus.Public)
                : 0
        });
    }

    #endregion

    #region -- Fields --

    private const int DefaultPageSize = 10;
    private const int MaxPageSize = 50;

    #endregion
}
