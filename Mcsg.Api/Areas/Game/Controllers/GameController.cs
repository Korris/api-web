using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mcsg.Api.Areas.Game.Controllers;

using Mcsg.Api.Areas.Game.Interfaces;
using Mcsg.Api.Areas.Game.Requests;

/// <summary>
/// Game post CRUD + list (api/game/game). Trimmed copy of Comic ComicController: no chapters / tags / reactions.
/// </summary>
[ApiController]
[Route("api/game/[controller]")]
public class GameController : ControllerBase
{
    #region -- Methods --

    public GameController(IGamePostService postService)
    {
        _postService = postService;
    }

    /// <summary>
    /// Create a game post. ThumbnailUrl / GameUrl must come from api/game/file upload endpoints.
    /// </summary>
    [HttpPost, Authorize]
    public async Task<IActionResult> Create([FromBody] GamePostCreateR request)
    {
        request.Analyze(HttpContext);
        var result = await _postService.CreateAsync(request);
        return Ok(result);
    }

    [HttpPut("{hashId}"), Authorize]
    public async Task<IActionResult> Update(string hashId, [FromBody] GamePostUpdateR request)
    {
        request.Analyze(HttpContext);
        request.HashId = hashId;
        var result = await _postService.UpdateAsync(request);
        return Ok(result);
    }

    [HttpDelete("{hashId}"), Authorize]
    public async Task<IActionResult> Delete(string hashId)
    {
        var request = new GameHashIdR { HashId = hashId };
        request.Analyze(HttpContext);
        var result = await _postService.DeleteAsync(request);
        return Ok(result);
    }

    /// <summary>
    /// Detail. Non-public posts are only visible to their owner.
    /// </summary>
    [HttpGet("{hashId}")]
    public async Task<IActionResult> Get(string hashId)
    {
        var request = new GameHashIdR { HashId = hashId };
        request.Analyze(HttpContext);
        var result = await _postService.GetAsync(request);
        return Ok(result);
    }

    /// <summary>
    /// Public list, newest first. Filters: ProfileName, Keyword. Mobile hides mature posts.
    /// </summary>
    [HttpGet("list")]
    public async Task<IActionResult> List([FromQuery] GamePostListR request)
    {
        request.Analyze(HttpContext);
        var result = await _postService.ListAsync(request);
        return Ok(result);
    }

    /// <summary>
    /// Current user's posts (all statuses)
    /// </summary>
    [HttpGet("my"), Authorize]
    public async Task<IActionResult> My([FromQuery] GamePostListR request)
    {
        request.Analyze(HttpContext);
        var result = await _postService.ListMineAsync(request);
        return Ok(result);
    }

    #endregion

    #region -- Fields --

    private readonly IGamePostService _postService;

    #endregion
}
