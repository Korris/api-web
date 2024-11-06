using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mcsg.Comic.Api.Controllers;

using Common.Core.Requests;
using Interfaces;
using Requests;

[ApiController]
[Route("[controller]")]
[Authorize]
public class FavoriteController : ControllerBase
{
    #region -- Methods --

    public FavoriteController(IFavoriteService favoriteService)
    {
        _favoriteService = favoriteService;
    }

    [HttpPost("add-post-favorite")]
    public async Task<IActionResult> AddPostToFavorite([FromBody] Guid postId)
    {
        var req = new IdBaseR(HttpContext) { Id = postId };
        var result = await _favoriteService.AddPostToFavoriteAsync(req);
        return Ok(result);
    }

    [HttpPost("add-tag-favorite")]
    public async Task<IActionResult> AddTagToFavorite([FromBody] Guid tagId)
    {
        var req = new IdBaseR(HttpContext) { Id = tagId };
        var result = await _favoriteService.AddTagToFavoriteAsync(req);
        return Ok(result);
    }

    [HttpGet("tag-favorite")]
    public async Task<IActionResult> GetTagFavorite([FromQuery] FavoriteTagR req)
    {
        var result = await _favoriteService.GetTagFavoriteAsync(req);
        return Ok(result);
    }

    [HttpGet("post-favorite")]
    public async Task<IActionResult> GetPostFavorite([FromQuery] FavoritePostR req)
    {
        req.Analyze(HttpContext);
        var result = await _favoriteService.GetPostFavoriteByUserAsync(req);
        return Ok(result);
    }

    [HttpDelete("remove-tag/{tagid}")]
    public async Task<IActionResult> RemoveTagFavorite(Guid tagid)
    {
        var req = new IdBaseR(HttpContext) { Id = tagid };
        var result = await _favoriteService.RemoveTagToFavoriteAsync(req);
        return Ok(result);
    }

    [HttpDelete("remove-post/{postId}")]
    public async Task<IActionResult> RemovePostFavorite(Guid postId)
    {
        var req = new IdBaseR(HttpContext) { Id = postId };
        var result = await _favoriteService.RemovePostToFavoriteAsync(req);
        return Ok(result);
    }

    #endregion

    #region -- Fields --

    private readonly IFavoriteService _favoriteService;

    #endregion
}
