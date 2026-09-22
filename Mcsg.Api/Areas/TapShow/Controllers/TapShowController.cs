using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mcsg.Api.Areas.TapShow.Controllers;

using Mcsg.Api.Areas.TapShow.Interfaces;
using Mcsg.Api.Areas.TapShow.Requests;

/// <summary>
/// TapShow post CRUD + list (api/tapshow/tapshow). Same shape as the Game GameController.
/// </summary>
[ApiController]
[Route("api/tapshow/[controller]")]
public class TapShowController : ControllerBase
{
    #region -- Methods --

    public TapShowController(ITapShowPostService postService)
    {
        _postService = postService;
    }

    /// <summary>
    /// Create a post. ThumbnailHashId must come from api/tapshow/file/upload-media.
    /// </summary>
    [HttpPost, Authorize]
    public async Task<IActionResult> Create([FromBody] TapShowPostCreateR request)
    {
        request.Analyze(HttpContext);
        return Ok(await _postService.CreateAsync(request));
    }

    [HttpPut("{hashId}"), Authorize]
    public async Task<IActionResult> Update(string hashId, [FromBody] TapShowPostUpdateR request)
    {
        request.Analyze(HttpContext);
        request.HashId = hashId;
        return Ok(await _postService.UpdateAsync(request));
    }

    /// <summary>
    /// Soft-delete the post with all its chapters / segments / choices; images are removed from storage
    /// </summary>
    [HttpDelete("{hashId}"), Authorize]
    public async Task<IActionResult> Delete(string hashId)
    {
        var request = new TapShowHashIdR { HashId = hashId };
        request.Analyze(HttpContext);
        return Ok(await _postService.DeleteAsync(request));
    }

    /// <summary>
    /// Detail. Non-public posts are only visible to their owner.
    /// </summary>
    [HttpGet("{hashId}")]
    public async Task<IActionResult> Get(string hashId)
    {
        var request = new TapShowHashIdR { HashId = hashId };
        request.Analyze(HttpContext);
        return Ok(await _postService.GetAsync(request));
    }

    /// <summary>
    /// Public list, newest first. Filters: ProfileName, Keyword. Mobile hides mature posts.
    /// </summary>
    [HttpGet("list")]
    public async Task<IActionResult> List([FromQuery] TapShowPostListR request)
    {
        request.Analyze(HttpContext);
        return Ok(await _postService.ListAsync(request));
    }

    /// <summary>
    /// Current user's posts (all statuses)
    /// </summary>
    [HttpGet("my"), Authorize]
    public async Task<IActionResult> My([FromQuery] TapShowPostListR request)
    {
        request.Analyze(HttpContext);
        return Ok(await _postService.ListMineAsync(request));
    }

    #endregion

    #region -- Fields --

    private readonly ITapShowPostService _postService;

    #endregion
}
