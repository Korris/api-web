using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mcsg.Comic.Api.Controllers;

using Common.Core.Enums;
using Common.Core.Requests;
using Interfaces;
using Requests;

[ApiController]
[Route("[controller]")]
public class ComicController : ControllerBase
{
    #region -- Methods --

    public ComicController(ISetting setting, IComicService comicService, IPostService postService, IPostReactService postReactService)
    {
        _setting = setting;
        _postService = postService;
        _comicService = comicService;
        _postReactService = postReactService;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> PostFeed(ComicPostCreateR request)
    {
        request.Analyze(HttpContext);
        var result = await _comicService.PostComic(request);
        return Ok(result);
    }

    [HttpGet("{hashId}")]
    public async Task<IActionResult> GetComic(string hashId, bool isLoadChapters = true)
    {
        var req = new ComicHashIdR { HashId = hashId, IsLoadChapters = isLoadChapters };
        req.Analyze(HttpContext);
        var result = await _comicService.GetComic(req);
        return Ok(result);
    }

    [HttpGet("top")]
    public async Task<IActionResult> GetTopComic()
    {
        var req = new BaseR(HttpContext);

        var result = await _comicService.GetTopComic();

        if (req.FromMobile)
        {
            result.TopCompleted = result.TopCompleted.Where(p => !p.IsMature).ToList();
            result.TopHits = result.TopHits.Where(p => !p.IsMature).ToList();
            result.TopLatest = result.TopLatest.Where(p => !p.IsMature).ToList();
        }

        return Ok(result);
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetAllTopComic([FromQuery] ComicPostListSeriesR request)
    {
        request.Analyze(HttpContext);
        var result = await _comicService.GetTopComicAsync(request);

        if (request.FromMobile)
        {
            result.Items = result.Items.Where(p => !p.IsMature).ToList();
        }

        return Ok(result);
    }

    [HttpGet("relation")]
    public async Task<IActionResult> GetRelationComics([FromQuery] ComicRelationPostSeriesR request)
    {
        var req = new BaseR();

        var result = await _comicService.GetRelationComicsAsync(request);

        if (req.FromMobile)
        {
            result.Items = result.Items.Where(p => !p.IsMature).ToList();
        }

        return Ok(result);
    }

    [HttpGet("top-hit")]
    public async Task<IActionResult> GetTopListHitComic([FromQuery] ComicTopPostR request)
    {
        var result = await _comicService.GetTopHitListComic(request);
        return Ok(result);
    }

    [HttpGet("top-latest")]
    public async Task<IActionResult> GetTopListLatestComic([FromQuery] ComicTopPostR request)
    {
        var result = await _comicService.GetTopLatestListComic(request);
        return Ok(result);
    }

    [HttpGet("top-completed")]
    public async Task<IActionResult> GetTopListCompletedComic([FromQuery] ComicTopPostR request)
    {
        var result = await _comicService.GetTopCompletedListComic(request);
        return Ok(result);
    }

    [HttpGet("recommended")]
    public async Task<IActionResult> GetRecommendedComic(int number)
    {
        var req = new ComicRecommendedR { Number = number };
        var result = await _comicService.GetRecommendedComic(req);
        return Ok(result);
    }

    [HttpPut("{hashId}")]
    [Authorize]
    public async Task<IActionResult> UpdateComic(string hashId, ComicPostUpdateR request)
    {
        request.Analyze(HttpContext);
        var result = await _comicService.UpdateComic(hashId, request);
        return Ok(result);
    }

    [HttpGet("{hashId}/chapter/{order}")]
    public async Task<IActionResult> GetChapter(string hashId, float order)
    {
        var result = await _comicService.GetChapter(hashId, order);
        return Ok(result);
    }

    [HttpGet("{comicHashId}/chapters")]
    public async Task<IActionResult> GetChapters(string comicHashId, [FromQuery] ComicChapterListR request)
    {
        var result = await _comicService.GetChapters(comicHashId, request);
        return Ok(result);
    }

    [HttpGet("{comicHashId}/chapters-list")]
    public async Task<IActionResult> GetChaptersList(string comicHashId)
    {
        var result = await _comicService.GetChaptersListSimple(comicHashId);
        return Ok(result);
    }

    [HttpPost("{comicHashId}/chapter")]
    [Authorize]
    public async Task<IActionResult> PostChapter(string comicHashId, ComicSubPostCreateR request)
    {
        request.Analyze(HttpContext);
        var result = await _comicService.PostChapterToComic(comicHashId, request);
        return Ok(result);
    }

    [HttpPut("{comicHashId}/chapter/{chapterOrder}")]
    [Authorize]
    public async Task<IActionResult> PutChapter(string comicHashId, int chapterOrder, ComicSubPostUpdateR request)
    {
        request.Analyze(HttpContext);
        var result = await _comicService.UpdateChapterToComic(comicHashId, chapterOrder, request);
        return Ok(result);
    }

    [HttpPut("{comicHashId}/chapter-swap")]
    [Authorize]
    public async Task<IActionResult> PutSwapChapter(string comicHashId, ComicChapterOrderSwapR orders)
    {
        var result = await _comicService.SwapChapterOrder(comicHashId, orders);
        return Ok(result);
    }

    [HttpDelete("{comicHashId}/chapter/{chapterOrder}")]
    [Authorize]
    public async Task<IActionResult> DeleteChapter(string comicHashId, int chapterOrder)
    {
        var result = await _comicService.DeleteChapter(comicHashId, chapterOrder);
        return Ok(result);
    }

    [HttpDelete("{postId}")]
    [Authorize]
    public async Task<IActionResult> DeleteFeed(Guid postId)
    {
        var result = await _comicService.Delete(postId);
        return Ok(result);
    }

    [HttpGet("my-comic")]
    [Authorize]
    public async Task<IActionResult> GetMyComic([FromQuery] ComicPostListSeriesR request)
    {
        request.Analyze(HttpContext);
        var result = await _comicService.GetMyComics(request);
        return Ok(result);
    }

    [HttpGet("search-by-profileName")]
    public async Task<IActionResult> GetSearchComic([FromQuery] ComicPostByProFileNameR input)
    {
        var result = await _comicService.GetComicByUserProfileName(input);
        return Ok(result);
    }

    [HttpGet("search-by-tagName")]
    public async Task<IActionResult> GetSearchComicByTagName([FromQuery] ComicPostByTagNameR input)
    {
        var result = await _comicService.GetComicByTagName(input);
        return Ok(result);
    }

    [HttpGet("get-followed-post")]
    public async Task<IActionResult> GetFollowedPost([FromQuery] PaginatedR input)
    {
        var result = await _comicService.GetFollowedPost(input);
        return Ok(result);
    }

    [Authorize]
    [HttpPost("follow-post/{postId}")]
    public async Task<IActionResult> FollowPost(Guid postId)
    {
        var result = await _comicService.FollowPost(postId);
        return Ok(result);
    }

    [HttpGet("post/{userName}")]
    public async Task<IActionResult> GetUserComic(string userName, [FromQuery] ComicTopPostR request)
    {
        request.Analyze(HttpContext);
        var result = await _postService.GetSeriesByUserByPage(PostType.Comic, userName, request);
        return Ok(result);
    }

    [HttpGet("latest-order")]
    public async Task<IActionResult> GetLatestOrder([FromQuery] string hashPostId)
    {
        var result = await _comicService.GetLatestOrderChapter(hashPostId);
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

    private readonly IComicService _comicService;

    private readonly IPostService _postService;

    private readonly IPostReactService _postReactService;

    #endregion
}
