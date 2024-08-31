using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mcsg.Story.Api.Controllers;

using Common.Core.Enums;
using Common.Core.Requests;
using Interfaces;
using Requests;

[ApiController]
[Route("[controller]")]
public class StoryController : ControllerBase
{
    #region -- Methods --

    public StoryController(ISetting setting, IStoryService storyService, IPostService postService, IPostReactService postReactService)
    {
        _setting = setting;
        _storyService = storyService;
        _postService = postService;
        _postReactService = postReactService;
    }

    #region -- Post --
    [HttpPost, Authorize]
    public async Task<IActionResult> PostCreate(StoryPostCreateR request)
    {
        request.Analyze(HttpContext);
        var result = await _storyService.PostCreate(request);
        return Ok(result);
    }

    [HttpPut("{hashId}"), Authorize]
    public async Task<IActionResult> PostUpdate(string hashId, StoryPostUpdateR request)
    {
        request.Analyze(HttpContext);
        var result = await _storyService.PostUpdate(hashId, request);
        return Ok(result);
    }
    #endregion

    #region -- SubPost --
    [HttpPost("{hashId}/chapter"), Authorize]
    public async Task<IActionResult> SubPostCreate(string hashId, StorySubPostCreateR request)
    {
        request.Analyze(HttpContext);
        var result = await _storyService.SubPostCreate(hashId, request);
        return Ok(result);
    }

    [HttpPut("{hashId}/chapter/{chapterOrder}"), Authorize]
    public async Task<IActionResult> SubPostUpdate(string hashId, int chapterOrder, StorySubPostUpdateR request)
    {
        request.Analyze(HttpContext);
        var result = await _storyService.SubPostUpdate(hashId, chapterOrder, request);
        return Ok(result);
    }
    #endregion

    [HttpGet("{hashId}")]
    public async Task<IActionResult> GetStory(string hashId, bool isLoadChapters = true)
    {
        var req = new StoryHashIdR { HashId = hashId, IsLoadChapters = isLoadChapters };
        req.Analyze(HttpContext);
        var result = await _storyService.Get(req);
        return Ok(result);
    }

    [HttpDelete("{postId}")]
    [Authorize]
    public async Task<IActionResult> DeleteFeed(Guid postId)
    {
        var result = await _storyService.Delete(postId);
        return Ok(result);
    }

    [HttpGet("top")]
    public async Task<IActionResult> GetTopStory()
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
    public async Task<IActionResult> GetAllTopStory([FromQuery] StoryPostListSeriesR request)
    {
        var req = new BaseR();
        request.Analyze(HttpContext);
        var result = await _storyService.GetTopAsync(request);

        if (req.FromMobile)
        {
            result.Items = result.Items.Where(p => !p.IsMature).ToList();
        }

        return Ok(result);
    }

    [HttpGet("relation")]
    public async Task<IActionResult> GetRelationStories([FromQuery] StoryRelationPostSeriesR request)
    {
        var req = new BaseR();

        var result = await _storyService.GetRelationAsync(request);

        if (req.FromMobile)
        {
            result.Items = result.Items.Where(p => !p.IsMature).ToList();
        }

        return Ok(result);
    }

    [HttpGet("top-hit")]
    public async Task<IActionResult> GetTopListHitStory([FromQuery] StoryTopPostR request)
    {
        var result = await _storyService.GetTopHitList(request);
        return Ok(result);
    }

    [HttpGet("top-latest")]
    public async Task<IActionResult> GetTopListLatestStory([FromQuery] StoryTopPostR request)
    {
        var result = await _storyService.GetTopLatestList(request);
        return Ok(result);
    }

    [HttpGet("top-completed")]
    public async Task<IActionResult> GetTopListCompletedStory([FromQuery] StoryTopPostR request)
    {
        var result = await _storyService.GetTopCompletedList(request);
        return Ok(result);
    }

    [HttpGet("{hashId}/chapters")]
    public async Task<IActionResult> GetChapters(string hashId, [FromQuery] StoryChapterListR request)
    {
        var result = await _storyService.GetChapters(hashId, request);
        return Ok(result);
    }

    [HttpGet("{hashId}/chapters-list")]
    public async Task<IActionResult> GetChaptersList(string hashId)
    {
        var result = await _storyService.GetChaptersListSimple(hashId);
        return Ok(result);
    }

    [HttpGet("{hashId}/chapter/{chapterOrder}")]
    public async Task<IActionResult> GetChapter(string hashId, float chapterOrder)
    {
        var result = await _storyService.GetChapter(hashId, chapterOrder);
        return Ok(result);
    }

    [HttpPut("{hashId}/chapter-swap")]
    [Authorize]
    public async Task<IActionResult> PutSwapChapter(string hashId, StoryChapterOrderSwapR orders)
    {
        var result = await _storyService.SwapChapterOrder(hashId, orders);
        return Ok(result);
    }

    [HttpDelete("{hashId}/chapter/{chapterOrder}")]
    [Authorize]
    public async Task<IActionResult> DeleteChapter(string hashId, int chapterOrder)
    {
        var result = await _storyService.DeleteChapter(hashId, chapterOrder);
        return Ok(result);
    }

    [HttpGet("my-stories")]
    [Authorize]
    public async Task<IActionResult> GetMyStories([FromQuery] StoryPostListSeriesR loadReq)
    {
        loadReq.Analyze(HttpContext);
        var result = await _storyService.GetMy(loadReq);
        return Ok(result);
    }

    [HttpGet("search-by-profileName")]
    public async Task<IActionResult> GetSearchComic([FromQuery] StoryPostByProFileNameR input)
    {
        var result = await _storyService.GetByUserProfileName(input);
        return Ok(result);
    }

    [HttpGet("search-by-tagName")]
    public async Task<IActionResult> GetSearchComicByTagName([FromQuery] StoryPostByTagNameR input)
    {
        var result = await _storyService.GetByTagName(input);
        return Ok(result);
    }

    [HttpGet("get-followed-post")]
    public async Task<IActionResult> GetFollowedPost([FromQuery] PaginatedR input)
    {
        var result = await _storyService.GetFollowedPost(input);
        return Ok(result);
    }

    [Authorize]
    [HttpPost("follow-post/{postId}")]
    public async Task<IActionResult> FollowPost(Guid postId)
    {
        var result = await _storyService.FollowPost(postId);
        return Ok(result);
    }

    [HttpGet("post/{userName}")]
    public async Task<IActionResult> GetUserStory(string userName, [FromQuery] StoryTopPostR loadReq)
    {
        loadReq.Analyze(HttpContext);
        var result = await _postService.GetSeriesByUserByPage(PostType.Story, userName, loadReq);
        return Ok(result);
    }

    [HttpGet("latest-order")]
    public async Task<IActionResult> GetLatestOrder([FromQuery] string hashPostId)
    {
        var result = await _storyService.GetLatestOrderChapter(hashPostId);
        return Ok(result);
    }

    [HttpGet("{id}/reactions")]
    public async Task<IActionResult> GetPostReactsByType(Guid id, [FromQuery] FeedReactionByTargetR request)
    {
        var result = await _postReactService.GetReactionsByTargetAsync(id, request);
        return Ok(result);
    }

    #endregion

    #region -- Fields --

    /// <summary>
    /// Setting
    /// </summary>
    private readonly ISetting _setting;

    private readonly IStoryService _storyService;

    private readonly IPostService _postService;

    private readonly IPostReactService _postReactService;

    #endregion
}
