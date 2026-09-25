using Microsoft.AspNetCore.Mvc;

namespace Mcsg.Api.Areas.TapShow.Comments;

using Common.Core.Requests;
using Common.Domain.Entities;

/// <summary>
/// TapShow post comments with the same routes / requests / responses as the Story CommentController (post comments only).
/// Create / update / delete / reply go through the realtime CommentHub with MicroService = "TapShow".
/// </summary>
[ApiController]
[Route("api/tapshow/comment")]
public class TapShowStoryCommentController : ControllerBase
{
    #region -- Methods --

    public TapShowStoryCommentController(ITapShowCommentReadService commentService, ITapShowReactionService<TapShowPostCommentReaction> commentReactService)
    {
        _commentService = commentService;
        _commentReactService = commentReactService;
    }

    [HttpGet("post/latest")]
    public async Task<IActionResult> GetPostLatestComment([FromQuery] Guid postId)
    {
        return Ok(await _commentService.GetLatestPostCommentInAsync(postId));
    }

    [HttpGet("post")]
    public async Task<IActionResult> GetPostComments([FromQuery] CommentLoadR request)
    {
        request.Analyze(HttpContext);
        return Ok(await _commentService.GetCommentsOfPostAsync(request));
    }

    [HttpGet("feed/latest")]
    public async Task<IActionResult> GetFeedLatestComment([FromQuery] Guid postId)
    {
        return Ok(await _commentService.GetLatestPostCommentInAsync(postId));
    }

    [HttpGet("feed")]
    public async Task<IActionResult> GetFeedComments([FromQuery] CommentLoadR request)
    {
        request.Analyze(HttpContext);
        return Ok(await _commentService.GetCommentsOfPostAsync(request));
    }

    [HttpGet("tapshow")]
    public async Task<IActionResult> GetTapShowComments([FromQuery] CommentLoadR request)
    {
        request.Analyze(HttpContext);
        return Ok(await _commentService.GetCommentsOfPostAsync(request));
    }

    [HttpGet("get-comment-by-id/{commentId}")]
    public async Task<IActionResult> GetCommentById(Guid commentId, [FromQuery] bool isSubPost, [FromQuery] Guid? replyCommentId)
    {
        var req = new BaseR(HttpContext);
        return Ok(await _commentService.GetCommentById(commentId, req.UserId, replyCommentId));
    }

    [HttpGet("comment-most-reaction")]
    public async Task<IActionResult> GetCommentWithMostReaction([FromQuery] CommentMostReactionR request)
    {
        request.Analyze(HttpContext);
        return Ok(await _commentService.GetCommentWithMostReaction(request));
    }

    [HttpGet("reply-by-comment")]
    public async Task<IActionResult> GetReplyByCommentId([FromQuery] CommentReplyByCommentR input)
    {
        input.Analyze(HttpContext);
        return Ok(await _commentService.GetReplyByCommentId(input));
    }

    [HttpGet("{id}/reactions")]
    public async Task<IActionResult> GetPostCommentReaction(Guid id, [FromQuery] FeedReactionByTargetR request)
    {
        request.Analyze(HttpContext);
        return Ok(await _commentReactService.GetReactionsByTargetAsync(id, request));
    }

    [HttpPatch("v1/CheckPostExisted")]
    public async Task<IActionResult> CheckPostExisted([FromBody] CommentCheckPostExistedR request)
    {
        request.Analyze(HttpContext);
        return Ok(await _commentService.CheckPostExisted(request));
    }

    #endregion

    #region -- Fields --

    private readonly ITapShowCommentReadService _commentService;
    private readonly ITapShowReactionService<TapShowPostCommentReaction> _commentReactService;

    #endregion
}
