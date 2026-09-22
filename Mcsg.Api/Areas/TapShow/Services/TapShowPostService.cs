using Microsoft.EntityFrameworkCore;

namespace Mcsg.Api.Areas.TapShow.Services;

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
/// TapShow post create / update / delete. Queries: TapShowPostService.Query.cs.
/// Delete cascades (soft) to chapters, segments, choices and releases every attached image.
/// </summary>
public partial class TapShowPostService : ITapShowPostService
{
    #region -- Methods --

    public TapShowPostService(IMcsgContext context, ITapShowResourceService resources, ITapShowReactService reactService)
    {
        _context = context;
        _resources = resources;
        _reactService = reactService;
    }

    public async Task<TapShowPostResponse> CreateAsync(TapShowPostCreateR request)
    {
        var vr = new TapShowPostCreateV().Validate(request);
        if (!vr.IsValid)
        {
            throw new BadRequestException(nameof(E000), vr.Errors.ToValue());
        }
        var userId = RequireUser(request.UserId);

        // Same flow as Game: the thumbnail was uploaded first, the post references it by hashId
        var thumbnail = await _resources.GetTempResourceAsync(request.ThumbnailHashId, userId);

        var post = TapShowPost.Create(request.Title, request.Summary, _resources.PublicUrl(thumbnail),
            request.IsCurrentUserAuthor ? request.ProfileName : request.AuthorName,
            request.IsCurrentUserAuthor ? userId : null,
            request.IsMature, request.IsCompleted, request.Permission, userId);

        await _context.TapShowPosts.AddAsync(post);
        _resources.Attach(thumbnail, post);
        await _context.SaveChangesAsync(default);

        return await GetByHashIdAsync(post.HashId, userId);
    }

    public async Task<TapShowPostResponse> UpdateAsync(TapShowPostUpdateR request)
    {
        var vr = new TapShowPostUpdateV().Validate(request);
        if (!vr.IsValid)
        {
            throw new BadRequestException(nameof(E000), vr.Errors.ToValue());
        }
        var userId = RequireUser(request.UserId);
        var post = await GetOwnedPostAsync(request.HashId, userId);

        // Same hashId as the attached thumbnail → keep it; new hashId → take the new upload and release the old one
        var current = post.TapShowResources.FirstOrDefault(r => !r.IsDelete && r.SegmentId == null && r.CharacterId == null);
        var thumbnail = current;
        if (current == null || current.HashId != request.ThumbnailHashId)
        {
            thumbnail = await _resources.GetTempResourceAsync(request.ThumbnailHashId, userId);
            if (current != null)
            {
                _resources.Release(current, userId);
            }
        }

        post.Update(request.Title, request.Summary, _resources.PublicUrl(thumbnail!),
            request.IsCurrentUserAuthor ? request.ProfileName : request.AuthorName,
            request.IsCurrentUserAuthor ? userId : null,
            request.IsMature, request.IsCompleted, request.Permission, userId);
        _resources.Attach(thumbnail!, post);
        await _context.SaveChangesAsync(default);

        await _resources.RemoveReleasedObjectsAsync();
        return await GetByHashIdAsync(post.HashId, userId);
    }

    public async Task<bool> DeleteAsync(TapShowHashIdR request)
    {
        var userId = RequireUser(request.UserId);
        var post = await GetOwnedPostAsync(request.HashId, userId);

        post.Delete(userId);

        // Soft-delete the whole tree: chapters → segments → choices
        var chapters = await _context.Available<TapShowChapter>().Where(c => c.PostId == post.Id).ToListAsync();
        var chapterIds = chapters.Select(c => c.Id).ToList();
        var segments = await _context.Available<TapShowSegment>().Where(s => chapterIds.Contains(s.ChapterId)).ToListAsync();
        var segmentIds = segments.Select(s => s.Id).ToList();
        var choices = await _context.Available<TapShowSegmentChoice>().Where(c => segmentIds.Contains(c.SegmentId)).ToListAsync();

        var characters = await _context.Available<TapShowCharacter>().Where(c => c.PostId == post.Id).ToListAsync();

        chapters.ForEach(c => c.Delete(userId));
        segments.ForEach(s => s.Delete(userId));
        choices.ForEach(c => c.Delete(userId));
        characters.ForEach(c => c.Delete(userId));

        // Every image of the post (thumbnail + segment images + character avatars)
        foreach (var resource in post.TapShowResources.Where(r => !r.IsDelete))
        {
            _resources.Release(resource, userId);
        }
        await _context.SaveChangesAsync(default);

        await _resources.RemoveReleasedObjectsAsync();
        return true;
    }

    #endregion

    #region -- Helpers --

    /// <summary>
    /// Logged-in user id or E109
    /// </summary>
    private static Guid RequireUser(Guid? userId)
    {
        if (userId == null || userId == Guid.Empty)
        {
            throw new BadRequestException(nameof(E109), E109);
        }
        return userId.Value;
    }

    /// <summary>
    /// Load a post that must exist (E204) and belong to the user (E309), with its attached files
    /// </summary>
    private async Task<TapShowPost> GetOwnedPostAsync(string? hashId, Guid userId)
    {
        var post = await _context.Available<TapShowPost>()
            .Include(p => p.TapShowResources)
            .FirstOrDefaultAsync(p => p.HashId == hashId);
        if (post == null)
        {
            throw new NotFoundException(nameof(E204), E204);
        }
        if (post.UserId != userId)
        {
            throw new ForbiddenAccessException(nameof(E309), E309);
        }
        return post;
    }

    #endregion

    #region -- Fields --

    private readonly IMcsgContext _context;
    private readonly ITapShowResourceService _resources;
    private readonly ITapShowReactService _reactService;

    #endregion
}
