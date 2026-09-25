using Microsoft.EntityFrameworkCore;

namespace Mcsg.Api.Areas.TapShow.Services;

using Common.Core.Enums;
using Common.Core.Extensions;
using Common.Domain;
using Common.Domain.Entities;
using Common.SeedWork.Exceptions;
using Mcsg.Api.Areas.TapShow.Interfaces;
using Mcsg.Api.Areas.TapShow.Models;
using Mcsg.Api.Areas.TapShow.Requests;
using Mcsg.Api.Areas.TapShow.Validators;
using static Common.SeedWork.Constants.Error;

/// <summary>
/// Comments on TapShow posts: create / reply (1 level) / update / delete. Queries in TapShowCommentService.Query.cs.
/// Copy of GameCommentService (REST only).
/// </summary>
public partial class TapShowCommentService : ITapShowCommentService
{
    #region -- Methods --

    public TapShowCommentService(IMcsgContext context, ITapShowReactService reactService)
    {
        _context = context;
        _reactService = reactService;
    }

    public async Task<CommentResponse> CreateAsync(CommentCreateR request)
    {
        var userId = RequireUser(request.UserId);
        ValidateBody(request.Body);

        // Same visibility rule as post detail: non-public status or Private permission → owner only
        var post = await _context.Available<TapShowPost>(false)
            .Where(p => p.HashId == request.PostHashId
                        && ((p.Status == PostStatus.Public && p.Permission != PostPermission.Private) || p.UserId == userId))
            .Select(p => new { p.Id })
            .FirstOrDefaultAsync();
        if (post == null)
        {
            throw new NotFoundException(nameof(E204), E204);
        }

        if (request.ParentId != null)
        {
            // Replies are one level deep: the parent must be a root comment of the same post
            var parentOk = await _context.Available<TapShowPostComment>(false)
                .AnyAsync(c => c.Id == request.ParentId && c.PostId == post.Id && c.ParentId == null && c.Status == CommentStatus.Public);
            if (!parentOk)
            {
                throw new NotFoundException(nameof(E204), E204);
            }
        }

        var comment = new TapShowPostComment
        {
            PostId = post.Id,
            ParentId = request.ParentId,
            AuthorId = userId,
            Body = request.Body!.Trim(),
            Order = 0,
            Status = CommentStatus.Public,
            CreatedBy = userId
        };
        await _context.TapShowPostComments.AddAsync(comment);
        await _context.SaveChangesAsync(default);

        return await GetByIdAsync(comment.Id, userId);
    }

    public async Task<CommentResponse> UpdateAsync(CommentUpdateR request)
    {
        var userId = RequireUser(request.UserId);
        ValidateBody(request.Body);

        var comment = await _context.Available<TapShowPostComment>().FirstOrDefaultAsync(c => c.Id == request.Id);
        if (comment == null)
        {
            throw new NotFoundException(nameof(E204), E204);
        }
        if (comment.AuthorId != userId)
        {
            throw new ForbiddenAccessException(nameof(E309), E309);
        }

        comment.Body = request.Body!.Trim();
        comment.ModifiedBy = userId;
        comment.ModifiedOn = DateTime.UtcNow;
        await _context.SaveChangesAsync(default);

        return await GetByIdAsync(comment.Id, userId);
    }

    /// <summary>
    /// Comment author or post owner may delete; deleting a root comment also hides its replies
    /// </summary>
    public async Task<bool> DeleteAsync(Guid id, Guid? currentUserId)
    {
        var userId = RequireUser(currentUserId);
        var comment = await _context.Available<TapShowPostComment>()
            .Include(c => c.Post)
            .FirstOrDefaultAsync(c => c.Id == id);
        if (comment == null)
        {
            throw new NotFoundException(nameof(E204), E204);
        }
        if (comment.AuthorId != userId && comment.Post.UserId != userId)
        {
            throw new ForbiddenAccessException(nameof(E309), E309);
        }

        var now = DateTime.UtcNow;
        comment.IsDelete = true;
        comment.ModifiedBy = userId;
        comment.ModifiedOn = now;

        if (comment.ParentId == null)
        {
            var replies = await _context.Available<TapShowPostComment>().Where(c => c.ParentId == id).ToListAsync();
            foreach (var reply in replies)
            {
                reply.IsDelete = true;
                reply.ModifiedBy = userId;
                reply.ModifiedOn = now;
            }
        }
        await _context.SaveChangesAsync(default);
        return true;
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

    private static void ValidateBody(string? body)
    {
        var vr = new CommentBodyV().Validate(body);
        if (!vr.IsValid)
        {
            throw new BadRequestException(nameof(E000), vr.Errors.ToValue());
        }
    }

    #endregion

    #region -- Fields --

    private readonly IMcsgContext _context;
    private readonly ITapShowReactService _reactService;

    #endregion
}
