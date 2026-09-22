using Microsoft.EntityFrameworkCore;

namespace Mcsg.Api.Areas.TapShow.Services;

using Common.Core.Enums;
using Common.Domain.Entities;
using Common.SeedWork.Exceptions;
using Mcsg.Api.Areas.TapShow.Models;
using Mcsg.Api.Areas.TapShow.Requests;
using static Common.SeedWork.Constants.Error;

/// <summary>
/// Chapter read queries: list of a post, detail with the segment graph. Visibility rules:
/// - post must be Public and not Private permission, unless the viewer owns it (404 otherwise)
/// - Draft chapters are owner-only
/// - Premium post + viewer neither premium nor owner → detail returned with IsLocked = true and no segments
/// </summary>
public partial class TapShowChapterService
{
    #region -- Methods --

    public async Task<List<ChapterResponse>> ListByPostAsync(PostHashIdR request)
    {
        var post = await _context.Available<TapShowPost>(false)
            .Where(p => p.HashId == request.PostHashId)
            .Select(p => new { p.Id, p.UserId, p.Status, p.Permission })
            .FirstOrDefaultAsync();
        var isOwner = post != null && request.UserId != null && post.UserId == request.UserId;
        if (post == null || (!isOwner && (post.Status != PostStatus.Public || post.Permission == PostPermission.Private)))
        {
            throw new NotFoundException(nameof(E204), E204);
        }

        var query = _context.Available<TapShowChapter>(false).Where(c => c.PostId == post.Id);
        if (!isOwner)
        {
            query = query.Where(c => c.Status == PostStatus.Public);
        }
        return await ProjectSummary(query.OrderBy(c => c.Order).ThenBy(c => c.CreatedOn)).ToListAsync();
    }

    public async Task<ChapterDetailResponse> GetAsync(TapShowHashIdR request)
    {
        // Visibility fields first (cheap), then the summary projection: keeps both queries translatable
        var chapter = await _context.Available<TapShowChapter>(false)
            .Where(c => c.HashId == request.HashId && !c.Post.IsDelete)
            .Select(c => new
            {
                c.Id,
                c.Status,
                PostUserId = c.Post.UserId,
                PostStatus = c.Post.Status,
                PostPermission = c.Post.Permission
            })
            .FirstOrDefaultAsync();

        var isOwner = chapter != null && request.UserId != null && chapter.PostUserId == request.UserId;
        if (chapter == null
            || (!isOwner && (chapter.PostStatus != PostStatus.Public || chapter.PostPermission == PostPermission.Private || chapter.Status != PostStatus.Public)))
        {
            throw new NotFoundException(nameof(E204), E204);
        }

        var summary = await GetSummaryAsync(chapter.Id);
        var result = new ChapterDetailResponse
        {
            Id = summary.Id,
            HashId = summary.HashId,
            PostId = summary.PostId,
            PostHashId = summary.PostHashId,
            Title = summary.Title,
            Order = summary.Order,
            Status = summary.Status,
            PublishDate = summary.PublishDate,
            CreatedOn = summary.CreatedOn,
            ModifiedOn = summary.ModifiedOn,
            SegmentCount = summary.SegmentCount,
            EndingCount = summary.EndingCount,
            IsLocked = !isOwner && chapter.PostPermission == PostPermission.Premium && !request.IsPremium
        };
        if (result.IsLocked)
        {
            return result;
        }

        result.Segments = await LoadSegmentsAsync(result.Id, isOwner);
        result.StartSegmentId = result.Segments.FirstOrDefault()?.Id;
        return result;
    }

    #endregion

    #region -- Helpers --

    private async Task<ChapterResponse> GetSummaryAsync(Guid chapterId)
    {
        return await ProjectSummary(_context.Available<TapShowChapter>(false).Where(c => c.Id == chapterId)).FirstOrDefaultAsync()
               ?? throw new NotFoundException(nameof(E204), E204);
    }

    /// <summary>
    /// Whole graph of a chapter ordered by Order (first = entry point) with Kind / NextSegmentId resolved.
    /// ImageHashId / AudioHashId are only exposed to the owner.
    /// </summary>
    private async Task<List<SegmentResponse>> LoadSegmentsAsync(Guid chapterId, bool isOwner)
    {
        var segments = await ProjectSegments(_context.Available<TapShowSegment>(false).Where(s => s.ChapterId == chapterId), isOwner)
            .ToListAsync();
        ResolveFlow(segments);
        return segments;
    }

    /// <summary>
    /// Kind: Choice (has choices) > Ending (flag) > Next. NextSegmentId = the following segment in the ordered list.
    /// </summary>
    internal static void ResolveFlow(List<SegmentResponse> ordered)
    {
        for (var i = 0; i < ordered.Count; i++)
        {
            var s = ordered[i];
            if (s.Choices.Count > 0)
            {
                s.Kind = SegmentKind.Choice;
            }
            else if (s.IsEnding)
            {
                s.Kind = SegmentKind.Ending;
            }
            else
            {
                s.Kind = SegmentKind.Next;
                s.NextSegmentId = i + 1 < ordered.Count ? ordered[i + 1].Id : null;
            }
        }
    }

    private static IQueryable<ChapterResponse> ProjectSummary(IQueryable<TapShowChapter> query)
    {
        return query.Select(c => new ChapterResponse
        {
            Id = c.Id,
            HashId = c.HashId!,
            PostId = c.PostId,
            PostHashId = c.Post.HashId,
            Title = c.Title,
            Order = c.Order,
            Status = c.Status,
            PublishDate = c.PublishDate,
            CreatedOn = c.CreatedOn,
            ModifiedOn = c.ModifiedOn,
            SegmentCount = c.TapShowSegments.Count(s => !s.IsDelete),
            EndingCount = c.TapShowSegments.Count(s => !s.IsDelete && s.IsEnding)
        });
    }

    /// <summary>
    /// Shared segment projection (also used by TapShowSegmentService)
    /// </summary>
    internal static IQueryable<SegmentResponse> ProjectSegments(IQueryable<TapShowSegment> query, bool isOwner)
    {
        return query
            .OrderBy(s => s.Order).ThenBy(s => s.CreatedOn)
            .Select(s => new SegmentResponse
            {
                Id = s.Id,
                ChapterId = s.ChapterId,
                Title = s.Title,
                ImageUrl = s.ImageUrl,
                ImageHashId = isOwner ? s.TapShowResources.Where(r => !r.IsDelete && r.Type == ResourceType.Image).Select(r => r.HashId).FirstOrDefault() : null,
                Narration = s.Narration,
                AudioUrl = s.AudioUrl,
                AudioHashId = isOwner ? s.TapShowResources.Where(r => !r.IsDelete && r.Type == ResourceType.Audio).Select(r => r.HashId).FirstOrDefault() : null,
                CharacterId = s.Character != null && !s.Character.IsDelete ? s.CharacterId : null,
                CharacterName = s.Character != null && !s.Character.IsDelete ? s.Character.Name : null,
                CharacterAvatarUrl = s.Character != null && !s.Character.IsDelete ? s.Character.AvatarUrl : null,
                Order = s.Order,
                IsEnding = s.IsEnding,
                CreatedOn = s.CreatedOn,
                ModifiedOn = s.ModifiedOn,
                Choices = s.Choices.Where(x => !x.IsDelete).OrderBy(x => x.Order).Select(x => new SegmentChoiceResponse
                {
                    Id = x.Id,
                    Label = x.Label,
                    Order = x.Order,
                    TargetSegmentId = x.TargetSegmentId
                }).ToList()
            });
    }

    #endregion
}
