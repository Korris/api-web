using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Mcsg.Api.Areas.Story.Controllers;

using Common.Core.Enums;
using Common.Core.Requests;
using Common.SeedWork.Responses;
using Mcsg.Api.Areas.Story.Interfaces;
using Mcsg.Api.Areas.Story.Requests;

[ApiController]
[Route("api/story/[controller]")]
public class StoryController : ControllerBase
{
    #region -- Methods --

    public StoryController(IMediator mediator, IStoryService storyService, IPostService postService, IPostReactService postReactService)
    {
        _mediator = mediator;
        _postService = postService;
        _storyService = storyService;
        _postReactService = postReactService;
    }

    #region -- Post --
    [HttpPost, Authorize]
    public async Task<IActionResult> PostCreate(StoryPostCreateR request)
    {
        request.Analyze(HttpContext);
        var result = await _postService.PostCreate(request);
        return Ok(result);
    }

    [HttpPut("{hashId}"), Authorize]
    public async Task<IActionResult> PostUpdate(string hashId, StoryPostUpdateR request)
    {
        request.Analyze(HttpContext);
        request.HashId = hashId;
        var result = await _postService.PostUpdate(request);
        return Ok(result);
    }
    #endregion

    #region -- SubPost --
    [HttpPost("{hashId}/chapter"), Authorize]
    public async Task<IActionResult> SubPostCreate(string hashId, StorySubPostCreateR request)
    {
        request.Analyze(HttpContext);
        request.PostHashId = hashId;
        var result = await _postService.SubPostCreate(request);
        return Ok(result);
    }

    [HttpPut("{hashId}/chapter/{chapterOrder}"), Authorize]
    public async Task<IActionResult> SubPostUpdate(string hashId, float chapterOrder, StorySubPostUpdateR request)
    {
        request.Analyze(HttpContext);
        request.PostHashId = hashId;
        request.ChapterOrder = chapterOrder;
        var result = await _postService.SubPostUpdate(request);
        return Ok(result);
    }
    #endregion

    [HttpGet("{hashId}")]
    public async Task<IActionResult> Get(string hashId, bool isLoadChapters = true)
    {
        var req = new StoryHashIdR { HashId = hashId, IsLoadChapters = isLoadChapters };
        req.Analyze(HttpContext);
        var result = await _storyService.Get(req);
        return Ok(result);
    }

    [HttpGet("top")]
    public async Task<IActionResult> GetTop()
    {
        var req = new BaseR(HttpContext);
        var result = await _storyService.GetTop();

        if (req.FromMobile)
        {
            result.TopCompleted = result.TopCompleted.Where(p => !p.IsMature).ToList();
            result.TopHits = result.TopHits.Where(p => !p.IsMature).ToList();
            result.TopLatest = result.TopLatest.Where(p => !p.IsMature).ToList();
        }

        return Ok(result);
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetAllTop([FromQuery] StoryPostListSeriesR request)
    {
        request.Analyze(HttpContext);
        var result = await _storyService.GetTopAsync(request);

        if (request.FromMobile)
        {
            result.Items = result.Items.Where(p => !p.IsMature).ToList();
        }

        return Ok(result);
    }

    [HttpGet("relation")]
    public async Task<IActionResult> GetRelation([FromQuery] StoryRelationPostSeriesR request)
    {
        request.Analyze(HttpContext);
        var result = await _storyService.GetRelationAsync(request);

        if (request.FromMobile)
        {
            result.Items = result.Items.Where(p => !p.IsMature).ToList();
        }

        return Ok(result);
    }

    [HttpGet("top-hit")]
    public async Task<IActionResult> GetTopHitList([FromQuery] StoryTopPostR request)
    {
        request.Analyze(HttpContext);
        var result = await _storyService.GetTopHitList(request);
        return Ok(result);
    }

    [HttpGet("top-latest")]
    public async Task<IActionResult> GetTopLatestList([FromQuery] StoryTopPostR request)
    {
        request.Analyze(HttpContext);
        var result = await _storyService.GetTopLatestList(request);
        return Ok(result);
    }

    [HttpGet("top-completed")]
    public async Task<IActionResult> GetTopCompletedList([FromQuery] StoryTopPostR request)
    {
        request.Analyze(HttpContext);
        var result = await _storyService.GetTopCompletedList(request);
        return Ok(result);
    }

    [HttpGet("{hashId}/chapter/{order}")]
    public async Task<IActionResult> GetChapter(string hashId, float order)
    {
        var req = new ChapterOrderR { HashId = hashId, Order = order };
        req.Analyze(HttpContext);
        var result = await _storyService.GetChapter(req);
        return Ok(result);
    }

    [Obsolete("This method is obsolete, please use v1/SubPost/Search")]
    [HttpGet("{hashId}/chapters")]
    public async Task<IActionResult> GetChapters(string hashId, [FromQuery] StoryChapterListR request)
    {
        request.Analyze(HttpContext);
        var result = await _storyService.GetChapters(hashId, request);
        return Ok(result);
    }

    [Authorize]
    [HttpGet("{hashId}/all-chapters")]
    public async Task<IActionResult> GetAllChapters(string hashId)
    {
        var result = await _storyService.GetAllChapters(hashId);
        return Ok(result);
    }

    [HttpGet("{hashId}/chapters-list")]
    public async Task<IActionResult> GetChaptersListSimple(string hashId)
    {
        var request = new StoryHashIdR { HashId = hashId };
        request.Analyze(HttpContext);
        var result = await _storyService.GetChaptersListSimple(request);
        return Ok(result);
    }

    [HttpPut("{hashId}/chapter-swap")]
    [Authorize]
    public async Task<IActionResult> SwapChapterOrder(string hashId, StoryChapterOrderSwapR orders)
    {
        orders.Analyze(HttpContext);
        var result = await _storyService.SwapChapterOrder(hashId, orders);
        return Ok(result);
    }

    [HttpPut("{hashId}/chapter-move")]
    [Authorize]
    public async Task<IActionResult> MoveChapterOrder(string hashId, StoryChapterOrderSwapR orders)
    {
        orders.Analyze(HttpContext);
        await _storyService.MoveChapterOrder(hashId, orders);
        return Ok();
    }

    [HttpDelete("{hashId}/chapter/{chapterOrder}"), Authorize]
    public async Task<IActionResult> DeleteChapter(string hashId, float chapterOrder)
    {
        var req = new BaseR(HttpContext);
        var result = await _postService.DeleteChapter(hashId, chapterOrder, req);
        return Ok(result);
    }

    [HttpDelete("{postId}"), Authorize]
    public async Task<IActionResult> Delete(Guid postId)
    {
        var req = new IdBaseR(HttpContext) { Id = postId };
        var result = await _postService.Delete(req);
        return Ok(result);
    }

    [HttpGet("my-stories")]
    [Authorize]
    public async Task<IActionResult> GetMy([FromQuery] StoryPostListSeriesR request)
    {
        request.Analyze(HttpContext);
        var result = await _storyService.GetMy(request);
        return Ok(result);
    }

    [HttpGet("search-by-profileName")]
    public async Task<IActionResult> GetByUserProfileName([FromQuery] StoryPostByProFileNameR input)
    {
        input.Analyze(HttpContext);
        var result = await _storyService.GetByUserProfileName(input);
        return Ok(result);
    }

    [HttpGet("search-by-tagName")]
    public async Task<IActionResult> GetByTagName([FromQuery] StoryPostByTagNameR input)
    {
        input.Analyze(HttpContext);
        var result = await _storyService.GetByTagName(input);
        return Ok(result);
    }

    [HttpGet("get-followed-post")]
    public async Task<IActionResult> GetFollowedPost([FromQuery] PaginatedR input)
    {
        input.Analyze(HttpContext);
        var result = await _storyService.GetFollowedPost(input);
        return Ok(result);
    }

    [Authorize]
    [HttpPost("follow-post/{postId}")]
    public async Task<IActionResult> FollowPost(Guid postId)
    {
        var req = new IdBaseR(HttpContext) { Id = postId };
        var result = await _storyService.FollowPost(req);
        return Ok(result);
    }

    [HttpGet("post/{userName}")]
    public async Task<IActionResult> GetSeriesByUserByPage(string userName, [FromQuery] StoryTopPostR request)
    {
        request.Analyze(HttpContext);
        var result = await _postService.GetSeriesByUserByPage(PostType.Story, userName, request);
        return Ok(result);
    }

    [HttpGet("latest-order")]
    public async Task<IActionResult> GetLatestOrderChapter([FromQuery] string hashPostId)
    {
        var result = await _storyService.GetLatestOrderChapter(hashPostId);
        return Ok(result);
    }

    [HttpGet("{id}/reactions")]
    public async Task<IActionResult> GetReactionsByTargetAsync(Guid id, [FromQuery] FeedReactionByTargetR request)
    {
        request.Analyze(HttpContext);
        var result = await _postReactService.GetReactionsByTargetAsync(id, request);
        return Ok(result);
    }

    /// <summary>
    /// ViewParagraph
    /// </summary>
    /// <returns>Return the result</returns>
    [HttpPatch("v1/ViewParagraph")]
    [ProducesResponseType(typeof(SingleResponse), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> ViewParagraph([FromForm] StoryPostViewParagraphR request)
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

    private readonly IStoryService _storyService;

    private readonly IPostService _postService;

    private readonly IPostReactService _postReactService;

    #endregion
}
