using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Mcsg.Api.Interfaces;

namespace Mcsg.Api.Areas.Social.Controllers;

using Common.Core.Enums;
using Common.Core.Requests;
using Common.SeedWork.Responses;
using Mcsg.Api.Areas.Social.Interfaces;
using Mcsg.Api.Areas.Social.Models;
using Mcsg.Api.Areas.Social.Requests;
using static Common.SeedWork.Constants.Setting;

[ApiController]
[Route("api/social/[controller]")]
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
    public async Task<IActionResult> GetTopListHitComic(PostType postType, string tagName, [FromQuery] PostTopR loadReq)
    {
        if (postType != PostType.Story && postType != PostType.Comic)
        {
            return NotFound();
        }
        var result = await _postService.GetSeriesByTagByPage(postType, tagName, loadReq);
        return Ok(result);
    }

    [HttpGet("latest-posts-by-type")]
    public async Task<IActionResult> GetLatestPostsByType()
    {
        var origin = Request.Headers["Origin"].ToString();
        Console.WriteLine($"Request Origin: {origin}");
        var req = new BaseR(HttpContext);
        var result = await _postService.GetLatestPostsByType(req);
        return Ok(result);
    }

    /// <summary>
    /// Same ranking as latest-posts-by-type, but every item already carries its Story/Comic/Document/Feed box.
    /// One call replaces latest-posts-by-type + the per-area get-*-by-list-id calls.
    /// </summary>
    [HttpGet("latest-posts-by-type/detail")]
    public async Task<IActionResult> GetLatestPostsByTypeWithDetail([FromServices] IHomeFeedAggregationService homeFeedAggregation)
    {
        // Injected per action: the aggregator pulls in the Story/Comic/Document/Feed services, no need to build them for every other post action
        var result = await homeFeedAggregation.GetLatestPostsWithDetail(HttpContext);
        return Ok(result);
    }

    [HttpGet("latest-posts-by-tag")]
    public async Task<IActionResult> GetLatestPostsByTag([FromQuery] string tagName)
    {
        var result = await _postService.GetLatestPostsByTag(tagName);
        return Ok(result);
    }

    /// <summary>
    /// Newest public posts across all content types (feed, story, comic, document), newest first.
    /// Returns ids/hashIds only, same shape as latest-posts-by-tag; fetch details with get-post-by-list-id.
    /// </summary>
    /// <param name="take">How many posts, default 4, max 50</param>
    [HttpGet("latest")]
    [ProducesResponseType(typeof(ListIdForHomePage), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetLatestPosts([FromQuery] int take = 4)
    {
        var result = await _postService.GetLatestPosts(take);
        return Ok(result);
    }

    /// <summary>
    /// Chapters (sub posts) scheduled for a future publish date across story, comic and document, soonest first.
    /// Titles and identifiers only, no images.
    /// </summary>
    /// <param name="take">How many rows, default 10, max 50</param>
    [HttpGet("upcoming")]
    [ProducesResponseType(typeof(List<UpcomingSubPostResponse>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetUpcomingSubPosts([FromQuery] int take = 10)
    {
        var result = await _postService.GetUpcomingSubPosts(take);
        return Ok(result);
    }

    /// <summary>
    /// Posts the caller is commenting on, most recently active first, across feed, story, comic and document.
    /// Each card carries title, total comments, comments newer than the caller's last one, and the newest comment
    /// with its author. The user comes from the token; anonymous callers get an empty list instead of 401 so the
    /// home page can call this together with the public endpoints.
    /// </summary>
    /// <param name="take">How many cards, default 10, max 50</param>
    [HttpGet("my-commented")]
    [ProducesResponseType(typeof(List<ActiveCommentPostResponse>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetMyCommentedPosts([FromQuery] int take = 10)
    {
        var req = new BaseR(HttpContext);
        if (req.UserId == null)
        {
            return Ok(new List<ActiveCommentPostResponse>());
        }

        var result = await _postService.GetActiveCommentPosts(req.UserId.Value, take);
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
        var req = new BaseR(HttpContext);
        var result = await _postService.GetPostDetails(hashIds, req);
        return Ok(result);
    }

    [HttpGet("get-post-maybe-you-like")]
    public async Task<IActionResult> GetPostMaybeYouLike([FromQuery] UserNamePagingR input)
    {
        input.Analyze(HttpContext);
        var result = await _postService.GetPostMaybeYouLike(input);
        return Ok(result);
    }

    [HttpGet("get-news-feed")]
    public async Task<IActionResult> GetNewsFeed([FromQuery] UserNamePagingR input)
    {
        input.Analyze(HttpContext);
        var result = await _postService.GetNewsFeed(input);
        return Ok(result);
    }

    [Authorize]
    [HttpGet("get-followed-post-count")]
    public async Task<IActionResult> GetFollowedPostCount()
    {
        var req = new BaseR(HttpContext);
        var result = await _postService.GetFollowedPostCount(req);
        return Ok(result);
    }

    /// <summary>
    /// SearchHashTag
    /// </summary>
    /// <param name="request">Request</param>
    /// <returns>Returns the result</returns>
    [HttpPatch("/SearchHashTag")]
    [ProducesResponseType(typeof(SingleResponse), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> SearchHashTag([FromBody] PostSearchHashTagR request)
    {
        request.Analyze(HttpContext);
        var response = await _mediator.Send(request);
        return Ok(response);
    }

    /// <summary>
    /// SyncToAna
    /// </summary>
    /// <param name="request">Request</param>
    /// <returns>Return the result</returns>
    [HttpPost("v1/SyncToAna"), Authorize(Policy = Policy.Admin)]
    [ProducesResponseType(typeof(SingleResponse), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> SyncToAna([FromBody] PostSyncToAnaR request)
    {
        request.Analyze(HttpContext);
        var response = await _mediator.Send(request);
        return Ok(response.Data);
    }

    /// <summary>
    /// PostHideUpdate
    /// </summary>
    /// <param name="request">Request</param>
    /// <returns>Return the result</returns>
    [HttpPost("v1/PostHideUpdate"), Authorize]
    [ProducesResponseType(typeof(SingleResponse), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> PostHideUpdate([FromBody] PostHideUpdateR request)
    {
        request.Analyze(HttpContext);
        var response = await _mediator.Send(request);
        return Ok(response.Data);
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
