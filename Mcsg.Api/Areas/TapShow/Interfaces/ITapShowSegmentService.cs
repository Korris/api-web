namespace Mcsg.Api.Areas.TapShow.Interfaces;

using Mcsg.Api.Areas.TapShow.Models;
using Mcsg.Api.Areas.TapShow.Requests;

/// <summary>
/// Segments (screens) of a chapter and their choices (branches). Owner only.
/// </summary>
public interface ITapShowSegmentService
{
    Task<SegmentResponse> CreateAsync(SegmentCreateR request);
    Task<SegmentResponse> UpdateAsync(SegmentUpdateR request);
    Task<bool> DeleteAsync(Guid id, Guid? userId);
    Task<SegmentResponse> SetChoicesAsync(SegmentChoicesSetR request);
}
