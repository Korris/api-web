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

    #region -- Post --
    [HttpPost, Authorize]
    public async Task<IActionResult> PostCreate(ComicPostCreateR request)
    {
        request.Analyze(HttpContext);
        var result = await _postService.PostCreate(request);
        return Ok(result);
    }

    [HttpPut("{hashId}"), Authorize]
    public async Task<IActionResult> PostUpdate(string hashId, ComicPostUpdateR request)
    {
        request.Analyze(HttpContext);
        request.HashId = hashId;
        var result = await _postService.PostUpdate(request);
        return Ok(result);
    }
    #endregion

    #region -- SubPost --
    [HttpPost("{hashId}/chapter"), Authorize]
    public async Task<IActionResult> SubPostCreate(string hashId, ComicSubPostCreateR request)
    {
        request.Analyze(HttpContext);
        request.PostHashId = hashId;
        var result = await _postService.SubPostCreate(request);
        return Ok(result);
    }

    [HttpPut("{hashId}/chapter/{chapterOrder}"), Authorize]
    public async Task<IActionResult> SubPostUpdate(string hashId, float chapterOrder, ComicSubPostUpdateR request)
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
        var req = new ComicHashIdR { HashId = hashId, IsLoadChapters = isLoadChapters };
        req.Analyze(HttpContext);
        var result = await _comicService.Get(req);
        return Ok(result);
    }

    [HttpGet("top")]
    public async Task<IActionResult> GetTop()
    {
        var req = new BaseR(HttpContext);
        var result = await _comicService.GetTop();

        if (req.FromMobile)
        {
            result.TopCompleted = result.TopCompleted.Where(p => !p.IsMature).ToList();
            result.TopHits = result.TopHits.Where(p => !p.IsMature).ToList();
            result.TopLatest = result.TopLatest.Where(p => !p.IsMature).ToList();
        }

        return Ok(result);
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetAllTop([FromQuery] ComicPostListSeriesR request)
    {
        request.Analyze(HttpContext);
        var result = await _comicService.GetTopAsync(request);

        if (request.FromMobile)
        {
            result.Items = result.Items.Where(p => !p.IsMature).ToList();
        }

        return Ok(result);
    }

    [HttpGet("relation")]
    public async Task<IActionResult> GetRelation([FromQuery] ComicRelationPostSeriesR request)
    {
        request.Analyze(HttpContext);
        var result = await _comicService.GetRelationAsync(request);

        if (request.FromMobile)
        {
            result.Items = result.Items.Where(p => !p.IsMature).ToList();
        }

        return Ok(result);
    }

    [HttpGet("top-hit")]
    public async Task<IActionResult> GetTopHitList([FromQuery] ComicTopPostR request)
    {
        request.Analyze(HttpContext);
        var result = await _comicService.GetTopHitList(request);
        return Ok(result);
    }

    [HttpGet("top-latest")]
    public async Task<IActionResult> GetTopLatestList([FromQuery] ComicTopPostR request)
    {
        request.Analyze(HttpContext);
        var result = await _comicService.GetTopLatestList(request);
        return Ok(result);
    }

    [HttpGet("top-completed")]
    public async Task<IActionResult> GetTopCompletedList([FromQuery] ComicTopPostR request)
    {
        request.Analyze(HttpContext);
        var result = await _comicService.GetTopCompletedList(request);
        return Ok(result);
    }

    [HttpGet("{hashId}/chapter/{order}")]
    public async Task<IActionResult> GetChapter(string hashId, float order)
    {
        var req = new ChapterOrderR { HashId = hashId, Order = order };
        req.Analyze(HttpContext);
        var result = await _comicService.GetChapter(req);
        return Ok(result);
    }

    [Obsolete("This method is obsolete, please use v1/SubPost/Search")]
    [HttpGet("{hashId}/chapters")]
    public async Task<IActionResult> GetChapters(string hashId, [FromQuery] ComicChapterListR request)
    {
        request.Analyze(HttpContext);
        var result = await _comicService.GetChapters(hashId, request);
        return Ok(result);
    }

    [Authorize]
    [HttpGet("{hashId}/all-chapters")]
    public async Task<IActionResult> GetAllChapters(string hashId)
    {
        var result = await _comicService.GetAllChapters(hashId);
        return Ok(result);
    }

    [HttpGet("{hashId}/chapters-list")]
    public async Task<IActionResult> GetChaptersListSimple(string hashId)
    {
        var request = new ComicHashIdR { HashId = hashId };
        request.Analyze(HttpContext);
        var result = await _comicService.GetChaptersListSimple(request);
        return Ok(result);
    }

    [HttpPut("{hashId}/chapter-swap")]
    [Authorize]
    public async Task<IActionResult> SwapChapterOrder(string hashId, ComicChapterOrderSwapR orders)
    {
        orders.Analyze(HttpContext);
        var result = await _comicService.SwapChapterOrder(hashId, orders);
        return Ok(result);
    }

    [HttpPut("{hashId}/chapter-move")]
    [Authorize]
    public async Task<IActionResult> MoveChapterOrder(string hashId, ComicChapterOrderSwapR orders)
    {
        orders.Analyze(HttpContext);
        await _comicService.MoveChapterOrder(hashId, orders);
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

    [HttpGet("my-comic")]
    [Authorize]
    public async Task<IActionResult> GetMy([FromQuery] ComicPostListSeriesR request)
    {
        request.Analyze(HttpContext);
        var result = await _comicService.GetMy(request);
        return Ok(result);
    }

    [HttpGet("search-by-profileName")]
    public async Task<IActionResult> GetByUserProfileName([FromQuery] ComicPostByProFileNameR input)
    {
        input.Analyze(HttpContext);
        var result = await _comicService.GetByUserProfileName(input);
        return Ok(result);
    }

    [HttpGet("search-by-tagName")]
    public async Task<IActionResult> GetByTagName([FromQuery] ComicPostByTagNameR input)
    {
        input.Analyze(HttpContext);
        var result = await _comicService.GetByTagName(input);
        return Ok(result);
    }

    [HttpGet("get-followed-post")]
    public async Task<IActionResult> GetFollowedPost([FromQuery] PaginatedR input)
    {
        input.Analyze(HttpContext);
        var result = await _comicService.GetFollowedPost(input);
        return Ok(result);
    }

    [Authorize]
    [HttpPost("follow-post/{postId}")]
    public async Task<IActionResult> FollowPost(Guid postId)
    {
        var req = new IdBaseR(HttpContext) { Id = postId };
        var result = await _comicService.FollowPost(req);
        return Ok(result);
    }

    [HttpGet("post/{userName}")]
    public async Task<IActionResult> GetSeriesByUserByPage(string userName, [FromQuery] ComicTopPostR request)
    {
        request.Analyze(HttpContext);
        var result = await _postService.GetSeriesByUserByPage(PostType.Comic, userName, request);
        return Ok(result);
    }

    [HttpGet("latest-order")]
    public async Task<IActionResult> GetLatestOrderChapter([FromQuery] string hashPostId)
    {
        var result = await _comicService.GetLatestOrderChapter(hashPostId);
        return Ok(result);
    }

    [HttpGet("{id}/reactions")]
    public async Task<IActionResult> GetReactionsByTargetAsync(Guid id, [FromQuery] FeedReactionByTargetR request)
    {
        request.Analyze(HttpContext);
        var result = await _postReactService.GetReactionsByTargetAsync(id, request);
        return Ok(result);
    }

    [HttpGet("recommended")]
    public async Task<IActionResult> GetRecommended(int number)
    {
        var req = new ComicRecommendedR { Number = number };
        var result = await _comicService.GetRecommended(req);
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
