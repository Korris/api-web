using Microsoft.EntityFrameworkCore;

namespace Mcsg.Api.Areas.TapShow.Services;

using Common.Core.Enums;
using Common.Core.Extensions;
using Common.Domain;
using Common.Domain.Entities;
using Common.SeedWork.Exceptions;
using Mcsg.Api.Areas.TapShow.Constants;
using Mcsg.Api.Areas.TapShow.Interfaces;
using Mcsg.Api.Areas.TapShow.Models;
using Mcsg.Api.Areas.TapShow.Requests;
using Mcsg.Api.Areas.TapShow.Validators;
using static Common.SeedWork.Constants.Error;

/// <summary>
/// Segment create / update / delete (owner only). Choices: TapShowSegmentService.Choices.cs.
/// </summary>
public partial class TapShowSegmentService : ITapShowSegmentService
{
    #region -- Methods --

    public TapShowSegmentService(IMcsgContext context, ITapShowResourceService resources)
    {
        _context = context;
        _resources = resources;
    }

    public async Task<SegmentResponse> CreateAsync(SegmentCreateR request)
    {
        var vr = new SegmentCreateV().Validate(request);
        if (!vr.IsValid)
        {
            throw new BadRequestException(nameof(E000), vr.Errors.ToValue());
        }
        var userId = RequireUser(request.UserId);

        var chapter = await _context.Available<TapShowChapter>()
            .Include(c => c.Post)
            .FirstOrDefaultAsync(c => c.HashId == request.ChapterHashId && !c.Post.IsDelete);
        if (chapter == null)
        {
            throw new NotFoundException(nameof(E204), E204);
        }
        if (chapter.Post.UserId != userId)
        {
            throw new ForbiddenAccessException(nameof(E309), E309);
        }

        await EnsureCharacterAsync(request.CharacterId, chapter.PostId);
        var image = string.IsNullOrWhiteSpace(request.ImageHashId) ? null : await _resources.GetTempResourceAsync(request.ImageHashId, userId, ResourceType.Image);
        var audio = string.IsNullOrWhiteSpace(request.AudioHashId) ? null : await _resources.GetTempResourceAsync(request.AudioHashId, userId, ResourceType.Audio);
        var order = request.Order ?? await NextOrderAsync(chapter.Id);

        var segment = TapShowSegment.Create(chapter.Id, request.CharacterId, request.Title?.Trim(), PublicUrlOrNull(image),
            request.Narration?.Trim(), PublicUrlOrNull(audio), order, request.IsEnding, userId);
        await _context.TapShowSegments.AddAsync(segment);
        AttachIfAny(image, chapter.Post, segment);
        AttachIfAny(audio, chapter.Post, segment);
        await _context.SaveChangesAsync(default);

        return await GetByIdAsync(segment.Id);
    }

    public async Task<SegmentResponse> UpdateAsync(SegmentUpdateR request)
    {
        var vr = new SegmentUpdateV().Validate(request);
        if (!vr.IsValid)
        {
            throw new BadRequestException(nameof(E000), vr.Errors.ToValue());
        }
        var userId = RequireUser(request.UserId);
        var segment = await GetOwnedSegmentAsync(request.Id, userId);
        await EnsureCharacterAsync(request.CharacterId, segment.Chapter.PostId);
        if (request.IsEnding && await _context.Available<TapShowSegmentChoice>(false).AnyAsync(c => c.SegmentId == segment.Id))
        {
            throw new BadRequestException(nameof(E000), TapShowConfig.EndingWithChoicesMessage);
        }

        // Image / audio: same hashId → keep; new hashId → swap; null → remove
        var image = await ResolveForUpdateAsync(segment, request.ImageHashId, ResourceType.Image, userId);
        var audio = await ResolveForUpdateAsync(segment, request.AudioHashId, ResourceType.Audio, userId);

        segment.Update(request.CharacterId, request.Title?.Trim(), PublicUrlOrNull(image), request.Narration?.Trim(), PublicUrlOrNull(audio),
            request.Order ?? segment.Order, request.IsEnding, userId);
        AttachIfAny(image, segment.Chapter.Post, segment);
        AttachIfAny(audio, segment.Chapter.Post, segment);
        await _context.SaveChangesAsync(default);

        await _resources.RemoveReleasedObjectsAsync();
        return await GetByIdAsync(segment.Id);
    }

    /// <summary>
    /// Soft-delete the segment, its outgoing and incoming choices, and release its image
    /// </summary>
    public async Task<bool> DeleteAsync(Guid id, Guid? currentUserId)
    {
        var userId = RequireUser(currentUserId);
        var segment = await GetOwnedSegmentAsync(id, userId);

        var choices = await _context.Available<TapShowSegmentChoice>()
            .Where(c => c.SegmentId == id || c.TargetSegmentId == id)
            .ToListAsync();
        choices.ForEach(c => c.Delete(userId));
        segment.Delete(userId);
        foreach (var resource in segment.TapShowResources.Where(r => !r.IsDelete))
        {
            _resources.Release(resource, userId);
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
    /// Segment that must exist (E204) and whose post belongs to the user (E309), with chapter, post and image loaded
    /// </summary>
    private async Task<TapShowSegment> GetOwnedSegmentAsync(Guid id, Guid userId)
    {
        var segment = await _context.Available<TapShowSegment>()
            .Include(s => s.Chapter).ThenInclude(c => c.Post)
            .Include(s => s.TapShowResources)
            .FirstOrDefaultAsync(s => s.Id == id && !s.Chapter.IsDelete && !s.Chapter.Post.IsDelete);
        if (segment == null)
        {
            throw new NotFoundException(nameof(E204), E204);
        }
        if (segment.Chapter.Post.UserId != userId)
        {
            throw new ForbiddenAccessException(nameof(E309), E309);
        }
        return segment;
    }

    /// <summary>
    /// Resource slot of a segment (image or audio) on update: same hashId as the attached file → keep it;
    /// new hashId → take the new temp upload and release the old one; null → release and return null.
    /// The temp upload is fetched before releasing, so a 400 mutates nothing.
    /// </summary>
    private async Task<TapShowResource?> ResolveForUpdateAsync(TapShowSegment segment, string? hashId, ResourceType type, Guid userId)
    {
        var current = segment.TapShowResources.FirstOrDefault(r => !r.IsDelete && r.Type == type);
        TapShowResource? next = null;
        if (!string.IsNullOrWhiteSpace(hashId))
        {
            next = current != null && current.HashId == hashId ? current : await _resources.GetTempResourceAsync(hashId, userId, type);
        }
        if (current != null && next != current)
        {
            _resources.Release(current, userId);
        }
        return next;
    }

    private void AttachIfAny(TapShowResource? resource, TapShowPost post, TapShowSegment segment)
    {
        if (resource != null)
        {
            _resources.Attach(resource, post, segment);
        }
    }

    private string? PublicUrlOrNull(TapShowResource? resource)
    {
        return resource == null ? null : _resources.PublicUrl(resource);
    }

    /// <summary>
    /// A referenced character must be a live character of the same post
    /// </summary>
    private async Task EnsureCharacterAsync(Guid? characterId, Guid postId)
    {
        if (characterId == null)
        {
            return;
        }
        var ok = await _context.Available<TapShowCharacter>(false).AnyAsync(c => c.Id == characterId && c.PostId == postId);
        if (!ok)
        {
            throw new BadRequestException(nameof(E000), TapShowConfig.InvalidCharacterMessage);
        }
    }

    /// <summary>
    /// Owner view of one segment with Kind / NextSegmentId resolved against its chapter siblings
    /// </summary>
    private async Task<SegmentResponse> GetByIdAsync(Guid id)
    {
        var chapterId = await _context.Available<TapShowSegment>(false).Where(s => s.Id == id).Select(s => (Guid?)s.ChapterId).FirstOrDefaultAsync()
                        ?? throw new NotFoundException(nameof(E204), E204);
        var siblings = await TapShowChapterService.ProjectSegments(_context.Available<TapShowSegment>(false).Where(s => s.ChapterId == chapterId), true)
            .ToListAsync();
        TapShowChapterService.ResolveFlow(siblings);
        return siblings.First(s => s.Id == id);
    }

    private async Task<float> NextOrderAsync(Guid chapterId)
    {
        var max = await _context.Available<TapShowSegment>(false)
            .Where(s => s.ChapterId == chapterId)
            .Select(s => (float?)s.Order)
            .MaxAsync();
        return (max ?? 0) + 1;
    }

    #endregion

    #region -- Fields --

    private readonly IMcsgContext _context;
    private readonly ITapShowResourceService _resources;

    #endregion
}
