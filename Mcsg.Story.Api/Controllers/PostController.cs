using Microsoft.AspNetCore.Mvc;

namespace Mcsg.Story.Api.Controllers;

using Common.Core.Enums;
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

    [HttpPost]
    public async Task<IActionResult> UpdateKeyWordForComicAndStoryToSmartLookup()
    {
        await _postService.UpdateKeyWordForComicAndStoryToSmartLookup();
        return Ok();
    }

    [HttpGet("latest-posts-by-type")]
    public async Task<IActionResult> GetLatestPostsByType()
    {
        var result = await _postService.GetLatestPostsByType();
        return Ok(result);
    }

    [HttpGet("latest-posts-by-tag")]
    public async Task<IActionResult> GetLatestPostsByTag([FromQuery] string tagName)
    {
        var result = await _postService.GetLatestPostsByTag(tagName);
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
        var result = await _postService.GetPostDetails(hashIds);
        return Ok(result);
    }

    [HttpGet("get-post-maybe-you-like")]
    public async Task<IActionResult> GetPostMaybeYouLike([FromQuery] UserNamePagingR input)
    {
        var result = await _postService.GetPostMaybeYouLike(input);
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
