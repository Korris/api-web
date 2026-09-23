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
/// Chapter create / update / delete (owner only). Queries: TapShowChapterService.Query.cs.
/// </summary>
public partial class TapShowChapterService : ITapShowChapterService
{
    #region -- Methods --

    public TapShowChapterService(IMcsgContext context, ITapShowResourceService resources)
    {
        _context = context;
        _resources = resources;
    }

    public async Task<ChapterResponse> CreateAsync(ChapterCreateR request)
    {
        var vr = new ChapterCreateV().Validate(request);
        if (!vr.IsValid)
        {
            throw new BadRequestException(nameof(E000), vr.Errors.ToValue());
        }
        var userId = RequireUser(request.UserId);

        var post = await _context.Available<TapShowPost>().FirstOrDefaultAsync(p => p.HashId == request.PostHashId)
                   ?? throw new NotFoundException(nameof(E204), E204);
        if (post.UserId != userId)
        {
            throw new ForbiddenAccessException(nameof(E309), E309);
        }

        // Null order → append after the last chapter
        var order = request.Order ?? await NextOrderAsync(post.Id);
        var thumbnail = string.IsNullOrWhiteSpace(request.ThumbnailHashId) ? null : await _resources.GetTempResourceAsync(request.ThumbnailHashId, userId);

        var chapter = TapShowChapter.Create(post, request.Title!.Trim(), thumbnail == null ? null : _resources.PublicUrl(thumbnail), order, request.Status, userId);
        await _context.TapShowChapters.AddAsync(chapter);
        if (thumbnail != null)
        {
            _resources.Attach(thumbnail, post, chapter: chapter);
        }
        await _context.SaveChangesAsync(default);

        return await GetSummaryAsync(chapter.Id, true);
    }

    public async Task<ChapterResponse> UpdateAsync(ChapterUpdateR request)
    {
        var vr = new ChapterUpdateV().Validate(request);
        if (!vr.IsValid)
        {
            throw new BadRequestException(nameof(E000), vr.Errors.ToValue());
        }
        var userId = RequireUser(request.UserId);
        var chapter = await GetOwnedChapterAsync(request.HashId, userId);

        // Thumbnail: same hashId → keep; new hashId → swap (old object deleted); null → remove
        var current = await GetThumbnailAsync(chapter.Id);
        var thumbnail = current;
        if (string.IsNullOrWhiteSpace(request.ThumbnailHashId))
        {
            thumbnail = null;
        }
        else if (current == null || current.HashId != request.ThumbnailHashId)
        {
            thumbnail = await _resources.GetTempResourceAsync(request.ThumbnailHashId, userId);
        }
        if (current != null && thumbnail != current)
        {
            _resources.Release(current, userId);
        }

        chapter.Update(request.Title!.Trim(), thumbnail == null ? null : _resources.PublicUrl(thumbnail), request.Order ?? chapter.Order, request.Status, userId);
        if (thumbnail != null)
        {
            _resources.Attach(thumbnail, chapter.Post, chapter: chapter);
        }
        await _context.SaveChangesAsync(default);

        await _resources.RemoveReleasedObjectsAsync();
        return await GetSummaryAsync(chapter.Id, true);
    }

    /// <summary>
    /// Soft-delete the chapter with its segments and choices; chapter thumbnail and segment images are released from the bucket
    /// </summary>
    public async Task<bool> DeleteAsync(TapShowHashIdR request)
    {
        var userId = RequireUser(request.UserId);
        var chapter = await GetOwnedChapterAsync(request.HashId, userId);

        var segments = await _context.Available<TapShowSegment>()
            .Include(s => s.TapShowResources)
            .Where(s => s.ChapterId == chapter.Id)
            .ToListAsync();
        var segmentIds = segments.Select(s => s.Id).ToList();
        var choices = await _context.Available<TapShowSegmentChoice>().Where(c => segmentIds.Contains(c.SegmentId)).ToListAsync();

        chapter.Delete(userId);
        var thumbnail = await GetThumbnailAsync(chapter.Id);
        if (thumbnail != null)
        {
            _resources.Release(thumbnail, userId);
        }
        choices.ForEach(c => c.Delete(userId));
        foreach (var segment in segments)
        {
            segment.Delete(userId);
            foreach (var resource in segment.TapShowResources.Where(r => !r.IsDelete))
            {
                _resources.Release(resource, userId);
            }
        }
        await _context.SaveChangesAsync(default);

        await _resources.RemoveReleasedObjectsAsync();
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

    /// <summary>
    /// Chapter that must exist (E204) and whose post belongs to the user (E309)
    /// </summary>
    private async Task<TapShowChapter> GetOwnedChapterAsync(string? hashId, Guid userId)
    {
        var chapter = await _context.Available<TapShowChapter>()
            .Include(c => c.Post)
            .FirstOrDefaultAsync(c => c.HashId == hashId && !c.Post.IsDelete);
        if (chapter == null)
        {
            throw new NotFoundException(nameof(E204), E204);
        }
        if (chapter.Post.UserId != userId)
        {
            throw new ForbiddenAccessException(nameof(E309), E309);
        }
        return chapter;
    }

    /// <summary>
    /// Live thumbnail of a chapter (resource attached via SubPostId), tracked
    /// </summary>
    private async Task<TapShowResource?> GetThumbnailAsync(Guid chapterId)
    {
        return await _context.TapShowResources.FirstOrDefaultAsync(r => r.SubPostId == chapterId && !r.IsDelete);
    }

    private async Task<float> NextOrderAsync(Guid postId)
    {
        var max = await _context.Available<TapShowChapter>(false)
            .Where(c => c.PostId == postId)
            .Select(c => (float?)c.Order)
            .MaxAsync();
        return (max ?? 0) + 1;
    }

    #endregion

    #region -- Fields --

    private readonly IMcsgContext _context;
    private readonly ITapShowResourceService _resources;

    #endregion
}
