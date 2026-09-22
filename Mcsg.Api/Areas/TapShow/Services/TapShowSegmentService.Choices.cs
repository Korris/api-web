using Microsoft.EntityFrameworkCore;

namespace Mcsg.Api.Areas.TapShow.Services;

using Common.Core.Extensions;
using Common.Domain.Entities;
using Common.SeedWork.Exceptions;
using Mcsg.Api.Areas.TapShow.Constants;
using Mcsg.Api.Areas.TapShow.Models;
using Mcsg.Api.Areas.TapShow.Requests;
using Mcsg.Api.Areas.TapShow.Validators;
using static Common.SeedWork.Constants.Error;

/// <summary>
/// Branching: replace the outgoing choices of a segment as a whole
/// </summary>
public partial class TapShowSegmentService
{
    #region -- Methods --

    /// <summary>
    /// Replace ALL choices of the segment. Targets must be live segments of the same chapter, distinct, and not the segment itself.
    /// A non-empty list makes the segment a Choice screen (IsEnding reset); an empty list makes it a plain Next segment.
    /// </summary>
    public async Task<SegmentResponse> SetChoicesAsync(SegmentChoicesSetR request)
    {
        var vr = new SegmentChoicesSetV().Validate(request);
        if (!vr.IsValid)
        {
            throw new BadRequestException(nameof(E000), vr.Errors.ToValue());
        }
        var userId = RequireUser(request.UserId);
        var segment = await GetOwnedSegmentAsync(request.Id, userId);

        var targetIds = request.Choices.Select(c => c.TargetSegmentId).ToList();
        if (targetIds.Contains(segment.Id))
        {
            throw new BadRequestException(nameof(E000), TapShowConfig.InvalidChoiceTargetMessage);
        }
        if (targetIds.Count > 0)
        {
            var found = await _context.Available<TapShowSegment>(false)
                .CountAsync(s => s.ChapterId == segment.ChapterId && targetIds.Contains(s.Id));
            if (found != targetIds.Count)
            {
                throw new BadRequestException(nameof(E000), TapShowConfig.InvalidChoiceTargetMessage);
            }
        }

        var existing = await _context.Available<TapShowSegmentChoice>().Where(c => c.SegmentId == segment.Id).ToListAsync();
        existing.ForEach(c => c.Delete(userId));

        var order = 0;
        foreach (var item in request.Choices)
        {
            await _context.TapShowSegmentChoices.AddAsync(
                TapShowSegmentChoice.Create(segment.Id, item.TargetSegmentId, item.Label!.Trim(), order++, userId));
        }
        // A segment with choices is a Choice screen, never an ending
        if (request.Choices.Count > 0)
        {
            segment.IsEnding = false;
        }
        segment.ModifiedBy = userId;
        segment.ModifiedOn = DateTime.UtcNow;
        await _context.SaveChangesAsync(default);

        return await GetByIdAsync(segment.Id);
    }

    #endregion
}
