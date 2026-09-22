using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mcsg.Api.Areas.TapShow.Controllers;

using Mcsg.Api.Areas.TapShow.Interfaces;
using Mcsg.Api.Areas.TapShow.Requests;

/// <summary>
/// Chapters of a TapShow post (api/tapshow/chapter). Owner manages them; readers load the segment graph via GET {hashId}.
/// </summary>
[ApiController]
[Route("api/tapshow/[controller]")]
public class ChapterController : ControllerBase
{
    #region -- Methods --

    public ChapterController(ITapShowChapterService chapterService)
    {
        _chapterService = chapterService;
    }

    /// <summary>
    /// Chapters of a post ordered by Order. Owner sees drafts too.
    /// </summary>
    [HttpGet("post/{postHashId}")]
    public async Task<IActionResult> ListByPost(string postHashId)
    {
        var request = new PostHashIdR { PostHashId = postHashId };
        request.Analyze(HttpContext);
        return Ok(await _chapterService.ListByPostAsync(request));
    }

    /// <summary>
    /// Chapter with its whole segment graph (segments + choices). Premium posts return IsLocked = true without segments
    /// for viewers who are neither premium nor the owner.
    /// </summary>
    [HttpGet("{hashId}")]
    public async Task<IActionResult> Get(string hashId)
    {
        var request = new TapShowHashIdR { HashId = hashId };
        request.Analyze(HttpContext);
        return Ok(await _chapterService.GetAsync(request));
    }

    [HttpPost, Authorize]
    public async Task<IActionResult> Create([FromBody] ChapterCreateR request)
    {
        request.Analyze(HttpContext);
        return Ok(await _chapterService.CreateAsync(request));
    }

    [HttpPut("{hashId}"), Authorize]
    public async Task<IActionResult> Update(string hashId, [FromBody] ChapterUpdateR request)
    {
        request.Analyze(HttpContext);
        request.HashId = hashId;
        return Ok(await _chapterService.UpdateAsync(request));
    }

    /// <summary>
    /// Soft-delete the chapter with its segments and choices
    /// </summary>
    [HttpDelete("{hashId}"), Authorize]
    public async Task<IActionResult> Delete(string hashId)
    {
        var request = new TapShowHashIdR { HashId = hashId };
        request.Analyze(HttpContext);
        return Ok(await _chapterService.DeleteAsync(request));
    }

    #endregion

    #region -- Fields --

    private readonly ITapShowChapterService _chapterService;

    #endregion
}
