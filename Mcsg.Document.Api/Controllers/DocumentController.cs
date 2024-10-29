using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mcsg.Document.Api.Controllers;

using Common.Core.Enums;
using Common.Core.Requests;
using Interfaces;
using Requests;

[ApiController]
[Route("[controller]")]
public class DocumentController : ControllerBase
{
    #region -- Methods --

    public DocumentController(ISetting setting, IDocumentService documentService, IPostService postService, IPostReactService postReactService)
    {
        _setting = setting;
        _postService = postService;
        _documentService = documentService;
        _postReactService = postReactService;
    }

    #region -- Post --
    [HttpPost, Authorize]
    public async Task<IActionResult> PostCreate(DocumentPostCreateR request)
    {
        request.Analyze(HttpContext);
        var result = await _postService.PostCreate(request);
        return Ok(result);
    }

    [HttpPut("{hashId}"), Authorize]
    public async Task<IActionResult> PostUpdate(string hashId, DocumentPostUpdateR request)
    {
        request.Analyze(HttpContext);
        request.HashId = hashId;
        var result = await _postService.PostUpdate(request);
        return Ok(result);
    }
    #endregion

    #region -- SubPost --
    [HttpPost("{hashId}/chapter"), Authorize]
    public async Task<IActionResult> SubPostCreate(string hashId, DocumentSubPostCreateR request)
    {
        request.Analyze(HttpContext);
        request.PostHashId = hashId;
        var result = await _postService.SubPostCreate(request);
        return Ok(result);
    }

    [HttpPut("{hashId}/chapter/{chapterOrder}"), Authorize]
    public async Task<IActionResult> SubPostUpdate(string hashId, float chapterOrder, DocumentSubPostUpdateR request)
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
        var req = new DocumentHashIdR { HashId = hashId, IsLoadChapters = isLoadChapters };
        req.Analyze(HttpContext);
        var result = await _documentService.Get(req);
        return Ok(result);
    }

    [HttpGet("top")]
    public async Task<IActionResult> GetTop()
    {
        var req = new BaseR(HttpContext);
        var result = await _documentService.GetTop();

        if (req.FromMobile)
        {
            result.TopCompleted = result.TopCompleted.Where(p => !p.IsMature).ToList();
            result.TopHits = result.TopHits.Where(p => !p.IsMature).ToList();
            result.TopLatest = result.TopLatest.Where(p => !p.IsMature).ToList();
        }

        return Ok(result);
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetAllTop([FromQuery] DocumentPostListSeriesR request)
    {
        request.Analyze(HttpContext);
        var result = await _documentService.GetTopAsync(request);

        if (request.FromMobile)
        {
            result.Items = result.Items.Where(p => !p.IsMature).ToList();
        }

        return Ok(result);
    }

    [HttpGet("relation")]
    public async Task<IActionResult> GetRelation([FromQuery] DocumentRelationPostSeriesR request)
    {
        request.Analyze(HttpContext);
        var result = await _documentService.GetRelationAsync(request);

        if (request.FromMobile)
        {
            result.Items = result.Items.Where(p => !p.IsMature).ToList();
        }

        return Ok(result);
    }

    [HttpGet("top-hit")]
    public async Task<IActionResult> GetTopHitList([FromQuery] DocumentTopPostR request)
    {
        var result = await _documentService.GetTopHitList(request);
        return Ok(result);
    }

    [HttpGet("top-latest")]
    public async Task<IActionResult> GetTopLatestList([FromQuery] DocumentTopPostR request)
    {
        var result = await _documentService.GetTopLatestList(request);
        return Ok(result);
    }

    [HttpGet("top-completed")]
    public async Task<IActionResult> GetTopCompletedList([FromQuery] DocumentTopPostR request)
    {
        var result = await _documentService.GetTopCompletedList(request);
        return Ok(result);
    }

    [HttpGet("{hashId}/chapter/{order}")]
    public async Task<IActionResult> GetChapter(string hashId, float order)
    {
        var req = new ChapterOrderR { HashId = hashId, Order = order };
        req.Analyze(HttpContext);
        var result = await _documentService.GetChapter(req);
        return Ok(result);
    }

    [HttpGet("{hashId}/chapters")]
    public async Task<IActionResult> GetChapters(string hashId, [FromQuery] DocumentChapterListR request)
    {
        var result = await _documentService.GetChapters(hashId, request);
        return Ok(result);
    }

    [Authorize]
    [HttpGet("{hashId}/all-chapters")]
    public async Task<IActionResult> GetAllChapters(string hashId)
    {
        var result = await _documentService.GetAllChapters(hashId);
        return Ok(result);
    }

    [HttpGet("{hashId}/chapters-list")]
    public async Task<IActionResult> GetChaptersListSimple(string hashId)
    {
        var result = await _documentService.GetChaptersListSimple(hashId);
        return Ok(result);
    }

    [HttpPut("{hashId}/chapter-swap")]
    [Authorize]
    public async Task<IActionResult> SwapChapterOrder(string hashId, DocumentChapterOrderSwapR orders)
    {
        var result = await _documentService.SwapChapterOrder(hashId, orders);
        return Ok(result);
    }

    [HttpPut("{hashId}/chapter-move")]
    [Authorize]
    public async Task<IActionResult> MoveChapterOrder(string hashId, DocumentChapterOrderSwapR orders)
    {
        await _documentService.MoveChapterOrder(hashId, orders);
        return Ok();
    }

    [HttpDelete("{hashId}/chapter/{chapterOrder}"), Authorize]
    public async Task<IActionResult> DeleteChapter(string hashId, float chapterOrder)
    {
        var result = await _postService.DeleteChapter(hashId, chapterOrder);
        return Ok(result);
    }

    [HttpDelete("{postId}"), Authorize]
    public async Task<IActionResult> Delete(Guid postId)
    {
        var result = await _postService.Delete(postId);
        return Ok(result);
    }

    [HttpGet("my-document")]
    [Authorize]
    public async Task<IActionResult> GetMy([FromQuery] DocumentPostListSeriesR request)
    {
        request.Analyze(HttpContext);
        var result = await _documentService.GetMy(request);
        return Ok(result);
    }

    [HttpGet("search-by-profileName")]
    public async Task<IActionResult> GetByUserProfileName([FromQuery] DocumentPostByProFileNameR input)
    {
        input.Analyze(HttpContext);
        var result = await _documentService.GetByUserProfileName(input);
        return Ok(result);
    }

    [HttpGet("search-by-tagName")]
    public async Task<IActionResult> GetByTagName([FromQuery] DocumentPostByTagNameR input)
    {
        input.Analyze(HttpContext);
        var result = await _documentService.GetByTagName(input);
        return Ok(result);
    }

    [HttpGet("get-followed-post")]
    public async Task<IActionResult> GetFollowedPost([FromQuery] PaginatedR input)
    {
        var result = await _documentService.GetFollowedPost(input);
        return Ok(result);
    }

    [Authorize]
    [HttpPost("follow-post/{postId}")]
    public async Task<IActionResult> FollowPost(FollowPostReq input)
    {
        var result = await _documentService.FollowPost(input);
        return Ok(result);
    }

    [HttpGet("post/{userName}")]
    public async Task<IActionResult> GetSeriesByUserByPage(string userName, [FromQuery] DocumentTopPostR request)
    {
        request.Analyze(HttpContext);
        var result = await _postService.GetSeriesByUserByPage(PostType.Document, userName, request);
        return Ok(result);
    }

    [HttpGet("latest-order")]
    public async Task<IActionResult> GetLatestOrderChapter([FromQuery] string hashPostId)
    {
        var result = await _documentService.GetLatestOrderChapter(hashPostId);
        return Ok(result);
    }

    [HttpGet("{id}/reactions")]
    public async Task<IActionResult> GetReactionsByTargetAsync(Guid id, [FromQuery] FeedReactionByTargetR request)
    {
        var result = await _postReactService.GetReactionsByTargetAsync(id, request);
        return Ok(result);
    }

    [HttpGet("recommended")]
    public async Task<IActionResult> GetRecommended(int number)
    {
        var req = new DocumentRecommendedR { Number = number };
        var result = await _documentService.GetRecommended(req);
        return Ok(result);
    }

    #endregion

    #region -- Fields --

    /// <summary>
    /// Setting
    /// </summary>
    private readonly ISetting _setting;

    private readonly IDocumentService _documentService;

    private readonly IPostService _postService;

    private readonly IPostReactService _postReactService;

    #endregion
}
