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

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> PostFeed(StoryPostCreateR request)
    {
        var result = await _storyService.PostStory(request);
        return Ok(result);
    }

    [HttpGet("{hashId}")]
    public async Task<IActionResult> GetStory(string hashId, bool isLoadChapters = true)
    {
        var req = new StoryHashIdR { HashId = hashId, IsLoadChapters = isLoadChapters };
        req.Analyze(HttpContext);
        var result = await _storyService.GetStory(req);
        return Ok(result);
    }

    [HttpPut("{hashId}")]
    [Authorize]
    public async Task<IActionResult> UpdateStory(string hashId, StoryPostUpdateR request)
    {
        var result = await _storyService.UpdateStory(hashId, request);
        return Ok(result);
    }

    [HttpPost("{hashId}/chapter")]
    [Authorize]
    public async Task<IActionResult> PostChapter(string hashId, StoryChapterR chapterPostReq)
    {
        var result = await _storyService.PostChapterToStory(hashId, chapterPostReq);
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

        var result = await _storyService.GetTopStory();

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
        var result = await _storyService.GetTopStoryAsync(request);

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

        var result = await _storyService.GetRelationStoriesAsync(request);

        if (req.FromMobile)
        {
            result.Items = result.Items.Where(p => !p.IsMature).ToList();
        }

        return Ok(result);
    }

    [HttpGet("top-hit")]
    public async Task<IActionResult> GetTopListHitStory([FromQuery] StoryTopPostR request)
    {
        var result = await _storyService.GetTopHitListStory(request);
        return Ok(result);
    }

    [HttpGet("top-latest")]
    public async Task<IActionResult> GetTopListLatestStory([FromQuery] StoryTopPostR request)
    {
        var result = await _storyService.GetTopLatestListStory(request);
        return Ok(result);
    }

    [HttpGet("top-completed")]
    public async Task<IActionResult> GetTopListCompletedStory([FromQuery] StoryTopPostR request)
    {
        var result = await _storyService.GetTopCompletedListStory(request);
        return Ok(result);
    }

    [HttpGet("{storyHashId}/chapters")]
    public async Task<IActionResult> GetChapters(string storyHashId, [FromQuery] StoryChapterListR request)
    {
        var result = await _storyService.GetChapters(storyHashId, request);
        return Ok(result);
    }

    [HttpGet("{storyHashId}/chapters-list")]
    public async Task<IActionResult> GetChaptersList(string storyHashId)
    {
        var result = await _storyService.GetChaptersListSimple(storyHashId);
        return Ok(result);
    }

    [HttpGet("{storyHashId}/chapter/{chapterOrder}")]
    public async Task<IActionResult> GetChapter(string storyHashId, float chapterOrder)
    {
        var result = await _storyService.GetChapter(storyHashId, chapterOrder);
        return Ok(result);
    }

    [HttpPut("{storyHashId}/chapter/{chapterOrder}")]
    [Authorize]
    public async Task<IActionResult> PutChapter(string storyHashId, int chapterOrder, StoryChapterR chapterPostReq)
    {
        var result = await _storyService.UpdateChapterToStory(storyHashId, chapterOrder, chapterPostReq);
        return Ok(result);
    }

    [HttpPut("{storyHashId}/chapter-swap")]
    [Authorize]
    public async Task<IActionResult> PutSwapChapter(string storyHashId, StoryChapterOrderSwapR orders)
    {
        var result = await _storyService.SwapChapterOrder(storyHashId, orders);
        return Ok(result);
    }

    [HttpDelete("{storyHashId}/chapter/{chapterOrder}")]
    [Authorize]
    public async Task<IActionResult> DeleteChapter(string storyHashId, int chapterOrder)
    {
        var result = await _storyService.DeleteChapter(storyHashId, chapterOrder);
        return Ok(result);
    }

    [HttpGet("my-stories")]
    [Authorize]
    public async Task<IActionResult> GetMyStories([FromQuery] StoryPostListSeriesR loadReq)
    {
        loadReq.Analyze(HttpContext);
        var result = await _storyService.GetMyStories(loadReq);
        return Ok(result);
    }

    [HttpGet("search-by-profileName")]
    public async Task<IActionResult> GetSearchComic([FromQuery] StoryPostByProFileNameR input)
    {
        var result = await _storyService.GetStoryByUserProfileName(input);
        return Ok(result);
    }

    [HttpGet("search-by-tagName")]
    public async Task<IActionResult> GetSearchComicByTagName([FromQuery] StoryPostByTagNameR input)
    {
        var result = await _storyService.GetStoryByTagName(input);
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
