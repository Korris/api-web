using Microsoft.AspNetCore.Mvc;

namespace Mcsg.Comic.Api.Controllers;

using Common.Core.Enums;
using Common.Core.Requests;
using Interfaces;
using Requests;

[ApiController]
[Route("[controller]")]
public class PostController : ControllerBase
{
    #region -- Methods --

    public PostController(IPostService postService)
    {
        _postService = postService;
    }

    [HttpGet("{postType}/tag/{tagName}")]
    public async Task<IActionResult> GetTopListHitComic(PostType postType, string tagName, [FromQuery] ComicTopPostR loadReq)
    {
        if (postType != PostType.Story && postType != PostType.Comic)
        {
            return NotFound();
        }
        var result = await _postService.GetSeriesByTagByPage(postType, tagName, loadReq);
        return Ok(result);
    }

    [HttpPost("get-random-ids")]
    public async Task<IActionResult> GetPostRandomIds([FromBody] PostRandomIdsR request)
    {
        var result = await _postService.GetPostRandomIdsAsync(request);
        return Ok(result);
    }

    [HttpPost("get-subpost-random-ids")]
    public async Task<IActionResult> GetSubPostRandomIdsAsync([FromBody] PostRandomIdsR request)
    {
        var result = await _postService.GetSubPostRandomIdsAsync(request);
        return Ok(result);
    }

    [HttpGet("get-post-by-list-id")]
    public async Task<IActionResult> GetPostDetails([FromQuery] string hashIds)
    {
        var req = new PaginatedR(HttpContext, hashIds);
        var result = await _postService.GetPostDetails(req);
        return Ok(result);
    }

    [HttpGet("get-news-feed")]
    public async Task<IActionResult> GetNewsFeed([FromQuery] UserNamePagingR input)
    {
        var result = await _postService.GetNewsFeed(input);
        return Ok(result);
    }

    #endregion

    #region -- Fields --

    private readonly IPostService _postService;

    #endregion
}
