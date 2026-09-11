using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Mcsg.Api.Areas.Comic.Controllers;

using Common.Core.Enums;
using Common.Core.Requests;
using Common.SeedWork.Responses;
using Mcsg.Api.Areas.Comic.Interfaces;
using Mcsg.Api.Interfaces;
using Mcsg.Api.Areas.Comic.Requests;
using static Common.SeedWork.Constants.Setting;

[ApiController]
[Route("api/comic/[controller]")]
public class PostController : ControllerBase
{
    #region -- Methods --

    public PostController(IMediator mediator, IPostService postService)
    {
        _mediator = mediator;
        _postService = postService;
    }

    [HttpGet("{postType}/tag/{tagName}")]
    public async Task<IActionResult> GetTopListHitComic(PostType postType, string tagName, [FromQuery] ComicTopPostR loadReq)
    {
        if (postType != PostType.Story && postType != PostType.Comic)
        {
            return NotFound();
        }
        loadReq.Analyze(HttpContext);
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
        input.Analyze(HttpContext);
        var result = await _postService.GetNewsFeed(input);
        return Ok(result);
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
    /// MyComicSearch
    /// </summary>
    /// <param name="request">Request</param>
    /// <returns>Return the result</returns>
    [HttpPatch("v1/MyComicSearch"), Authorize]
    [ProducesResponseType(typeof(SingleResponse), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> MyComicSearch([FromBody] MyComicSearchR request)
    {
        request.Analyze(HttpContext);
        var response = await _mediator.Send(request);
        return Ok(response);
    }

    /// <summary>
    /// LoadFeed (ported from api-mobile PATCH v1/Post/LoadFeed)
    /// </summary>
    /// <param name="request">Request</param>
    /// <returns>Return the result</returns>
    [HttpPatch("v1/LoadFeed")]
    [ProducesResponseType(typeof(SingleResponse), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> LoadFeed([FromBody] PostLoadFeedR request)
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

    private readonly IPostService _postService;

    #endregion
}
