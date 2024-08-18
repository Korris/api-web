using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Mcsg.Social.Api.Controllers;

using Common.Core.Enums;
using Common.SeedWork.Responses;
using Interfaces;
using Microsoft.AspNetCore.Authorization;
using Requests;

[ApiController]
[Route("[controller]")]
public class PostController : ControllerBase
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="mediator">Mediator</param>
    /// <param name="setting">Setting</param>
    /// <param name="postService">Post Service</param>
    public PostController(IMediator mediator, ISetting setting, IPostService postService)
    {
        _mediator = mediator;
        _setting = setting;
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

    [Authorize]
    [HttpGet("get-followed-post-count")]
    public async Task<IActionResult> GetFollowedPostCount()
    {
        var result = await _postService.GetFollowedPostCount();
        return Ok(result);
    }

    /// <summary>
    /// SearchHashTag
    /// </summary>
    /// <param name="request">Request</param>
    /// <returns>Returns the result</returns>
    [HttpPatch("/v1/SearchHashTag")]
    [ProducesResponseType(typeof(SingleResponse), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> SearchHashTag([FromBody] PostSearchHashTagR request)
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

    /// <summary>
    /// Setting
    /// </summary>
    private readonly ISetting _setting;

    /// <summary>
    /// Post service
    /// </summary>
    private readonly IPostService _postService;

    #endregion
}
