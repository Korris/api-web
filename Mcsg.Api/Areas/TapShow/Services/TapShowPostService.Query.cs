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
/// TapShow post read queries (detail / list / my list) with Permission enforcement, chapter + comment counts and reactions
/// </summary>
public partial class TapShowPostService
{
    #region -- Methods --

    public Task<TapShowPostResponse> GetAsync(TapShowHashIdR request)
    {
        return GetByHashIdAsync(request.HashId, request.UserId);
    }

    public Task<PagedResponse<TapShowPostResponse>> ListAsync(TapShowPostListR request)
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

    public Task<PagedResponse<TapShowPostResponse>> ListMineAsync(TapShowPostListR request)
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
    /// Premium posts stay visible; the chapter content is what gets locked (see TapShowChapterService).
    /// </summary>
    private async Task<TapShowPostResponse> GetByHashIdAsync(string? hashId, Guid? currentUserId)
    {
        var post = await Project(BaseQuery(), currentUserId).FirstOrDefaultAsync(p => p.HashId == hashId);

        var isOwner = post != null && currentUserId != null && post.UserId == currentUserId;
        if (post == null || (!isOwner && (post.Status != PostStatus.Public || post.Permission == PostPermission.Private)))
        {
            throw new NotFoundException(nameof(E204), E204);
        }
        await AttachReactionsAsync(new List<TapShowPostResponse> { post }, currentUserId);
        return post;
    }

    private IQueryable<TapShowPost> BaseQuery()
    {
        return _context.Available<TapShowPost>(false);
    }

    private async Task<PagedResponse<TapShowPostResponse>> PageAsync(IQueryable<TapShowPost> query, TapShowPostListR request)
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
        return new PagedResponse<TapShowPostResponse>(items, total, pageNumber, pageSize);
    }

    /// <summary>
    /// One grouped reaction query for the whole page
    /// </summary>
    private async Task AttachReactionsAsync(List<TapShowPostResponse> items, Guid? currentUserId)
    {
        var summaries = await _reactService.GetPostSummariesAsync(items.Select(p => p.Id), currentUserId);
        foreach (var item in items)
        {
            item.Reaction = summaries.TryGetValue(item.Id, out var summary) ? summary : new ReactionSummaryResponse { TargetId = item.Id };
        }
    }

    /// <summary>
    /// Entity → response projection (single place so detail and list return the same shape).
    /// ChapterCount: the owner sees every chapter, others only Public ones.
    /// </summary>
    private static IQueryable<TapShowPostResponse> Project(IQueryable<TapShowPost> query, Guid? currentUserId)
    {
        return query.Select(p => new TapShowPostResponse
        {
            Id = p.Id,
            HashId = p.HashId!,
            Title = p.Title,
            Body = p.Body,
            ThumbnailUrl = p.ThumbnailUrl,
            AuthorName = p.AuthorName,
            IsCurrentUserAuthor = currentUserId != null && p.UserId == currentUserId,
            IsMature = p.IsMature == true,
            IsCompleted = p.IsCompleted == true,
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
            ChapterCount = p.TapShowChapters.Count(c => !c.IsDelete && (c.Status == PostStatus.Public || (currentUserId != null && p.UserId == currentUserId))),
            CommentCount = p.TapShowPostComments.Count(c => !c.IsDelete && c.Status == CommentStatus.Public)
        });
    }

    #endregion

    #region -- Fields --

    private const int DefaultPageSize = 10;
    private const int MaxPageSize = 50;

    #endregion
}
