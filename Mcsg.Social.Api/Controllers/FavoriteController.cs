using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mcsg.Social.Api.Controllers;

using Common.Core.Requests;
using Interfaces;
using Requests;

[ApiController]
[Route("[controller]")]
[Authorize]
public class FavoriteController : ControllerBase
{
    #region -- Methods --

    public FavoriteController(IMediator mediator, IFavoriteService favoriteService)
    {
        _mediator = mediator;
        _favoriteService = favoriteService;
    }

    [HttpPost("add-post-favorite")]
    public async Task<IActionResult> AddPostToFavorite([FromBody] Guid postId)
    {
        var request = new PostFavoriteUpdateR { PostId = postId };
        request.Analyze(HttpContext);
        var response = await _mediator.Send(request);
        return Ok(response.Data);
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
        req.Analyze(HttpContext);
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

    /// <summary>
    /// View
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPatch("View")]
    public async Task<IActionResult> View([FromBody] FavoriteViewR request)
    {
        request.Analyze(HttpContext);
        var response = await _mediator.Send(request);
        return Ok(response.Data);
    }

    /// <summary>
    /// Search
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPatch("Search")]
    public async Task<IActionResult> Search([FromBody] FavoriteSearchR request)
    {
        request.Analyze(HttpContext);

        var response = await _mediator.Send(request);

        return Ok(response);
    }

    #endregion

    #region -- Fields --

    /// <summary>
    /// Mediator
    /// </summary>
    private readonly IMediator _mediator;

    private readonly IFavoriteService _favoriteService;

    #endregion
}
