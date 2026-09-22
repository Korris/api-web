using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mcsg.Api.Areas.TapShow.Controllers;

using Mcsg.Api.Areas.TapShow.Interfaces;
using Mcsg.Api.Areas.TapShow.Requests;

/// <summary>
/// Segments (screens) of a chapter and their branching choices (api/tapshow/segment). Owner only.
/// </summary>
[ApiController]
[Route("api/tapshow/[controller]"), Authorize]
public class SegmentController : ControllerBase
{
    #region -- Methods --

    public SegmentController(ITapShowSegmentService segmentService)
    {
        _segmentService = segmentService;
    }

    /// <summary>
    /// Create a segment. ImageHashId (optional) must come from api/tapshow/file/upload-media.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] SegmentCreateR request)
    {
        request.Analyze(HttpContext);
        return Ok(await _segmentService.CreateAsync(request));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] SegmentUpdateR request)
    {
        request.Analyze(HttpContext);
        request.Id = id;
        return Ok(await _segmentService.UpdateAsync(request));
    }

    /// <summary>
    /// Soft-delete the segment; choices pointing to it are removed as well
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var request = new SegmentUpdateR();
        request.Analyze(HttpContext);
        return Ok(await _segmentService.DeleteAsync(id, request.UserId));
    }

    /// <summary>
    /// Replace all choices (branches) of the segment. Empty list = ending.
    /// </summary>
    [HttpPut("{id}/choices")]
    public async Task<IActionResult> SetChoices(Guid id, [FromBody] SegmentChoicesSetR request)
    {
        request.Analyze(HttpContext);
        request.Id = id;
        return Ok(await _segmentService.SetChoicesAsync(request));
    }

    #endregion

    #region -- Fields --

    private readonly ITapShowSegmentService _segmentService;

    #endregion
}
