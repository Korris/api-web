using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mcsg.Social.Api.Controllers;

using Common.Core.Requests;
using Enums;
using Interfaces;
using Requests;

[ApiController]
[Route("[controller]")]
public class FeedController : ControllerBase
{
    #region -- Methods --

    public FeedController(IMediator mediator, IFeedService feedService, IPostReactService postReactService)
    {
        _mediator = mediator;
        _feedService = feedService;
        _postReactService = postReactService;
    }

    [HttpPost, Authorize]
    public async Task<IActionResult> PostFeed([FromBody] PostCreateR request)
    {
        request.Analyze(HttpContext);
        var response = await _mediator.Send(request);
        return Ok(response.Data);
    }

    [HttpPut("{hashId}"), Authorize]
    public async Task<IActionResult> UpdateFeed(string hashId, [FromBody] PostUpdateR request)
    {
        request.Analyze(HttpContext);
        request.HashId = hashId;
        var response = await _mediator.Send(request);
        return Ok(response.Data);
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetFeeds([FromQuery] FeedLoadReq request)
    {
        request.Analyze(HttpContext);
        var result = await _feedService.GetFeedsAsync(request, Enums.LoadFeedType.ALL);
        return Ok(result);
    }

    [HttpGet("display-setting")]
    public IActionResult GetFeedDisplayConfig()
    {
        var result = _feedService.GetFeedDisplayConfig();
        return Ok(result);
    }

    [HttpGet("trending")]
    public async Task<IActionResult> GetTredingFeeds([FromQuery] FeedLoadReq request)
    {
        var result = await _feedService.GetFeedsAsync(request, Enums.LoadFeedType.TRENDING);
        return Ok(result);
    }

    [HttpGet("hot")]
    public async Task<IActionResult> GetHotFeeds([FromQuery] FeedLoadReq request)
    {
        var result = await _feedService.GetFeedsAsync(request, Enums.LoadFeedType.HOT);
        return Ok(result);
    }

    [HttpGet("list/{tagName}")]
    public async Task<IActionResult> GetFeedByTag(string tagName, [FromQuery] FeedLoadReq request)
    {
        var result = await _feedService.GetFeedsByTagAsync(tagName, request);
        return Ok(result);
    }

    [HttpGet("search/{keyword}")]
    public async Task<IActionResult> GetFeedByKeyword(string keyword, [FromQuery] FeedSearchKeywordR request)
    {
        var result = await _feedService.GetFeedByKeywordAsync(keyword, request);
        return Ok(result);
    }

    [HttpGet("{hashId}")]
    public async Task<IActionResult> GetFeed(string hashId)
    {
        var req = new BaseR(HttpContext);
        var result = await _feedService.GetFeedAsync(hashId, req.UserId ?? Guid.Empty);
        return Ok(result);
    }

    [HttpGet("subpost/{hashId}")]
    public async Task<IActionResult> GetFeedSubPost(string hashId)
    {
        var req = new BaseR(HttpContext);
        var result = await _feedService.GetFeedSubPostAsync(hashId, req.UserId ?? Guid.Empty);
        return Ok(result);
    }

    [HttpDelete("{postId}")]
    [Authorize]
    public async Task<IActionResult> DeleteFeed(Guid postId)
    {
        var result = await _feedService.DeleteFeedAsync(postId);
        return Ok(result);
    }

    [HttpGet("{id}/reactions")]
    public async Task<IActionResult> GetPostReactsByType(Guid id, [FromQuery] FeedReactionByTargetR request)
    {
        var result = await _postReactService.GetReactionsByTargetAsync(id, request);
        return Ok(result);
    }

    [HttpPost("report")]
    [Authorize]
    public async Task<IActionResult> ReportFeed([FromBody] FeedReportPostReq request)
    {
        var result = await _feedService.ReportFeedAsync(request);
        return Ok(result);
    }

    [HttpGet("get-feed-by-list-id")]
    public async Task<IActionResult> GetFeedsByIds([FromQuery] string hashIds)
    {
        var req = new BaseR(HttpContext);
        req.Analyze(HttpContext);
        var result = await _feedService.GetFeedsByIds(hashIds, req.UserId ?? Guid.Empty);
        return Ok(result);
    }

    [HttpGet("post/{userName}")]
    public async Task<IActionResult> GetUserFeed(string userName, [FromQuery] FeedLoadReq loadReq)
    {
        loadReq.UserName = userName;
        var result = await _feedService.GetFeedsAsync(loadReq, LoadFeedType.ALL);
        return Ok(result);
    }

    #endregion

    #region -- Fields --

    /// <summary>
    /// Mediator
    /// </summary>
    private readonly IMediator _mediator;

    private readonly IFeedService _feedService;

    private readonly IPostReactService _postReactService;

    #endregion
}
