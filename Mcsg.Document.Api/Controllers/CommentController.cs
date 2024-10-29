using Microsoft.AspNetCore.Mvc;

namespace Mcsg.Document.Api.Controllers;

using Common.Core.Enums;
using Common.Core.Requests;
using Interfaces;
using Requests;

[ApiController]
[Route("[controller]")]
public class CommentController : ControllerBase
{
    #region -- Methods --

    public CommentController(ICommentService commentService, IPostCommentReactService postCommentReactService, ISubPostCommentReactService subPostCommentReactService)
    {
        _commentService = commentService;
        _postCommentReactService = postCommentReactService;
        _subPostCommentReactService = subPostCommentReactService;
    }

    #region Should remove after FE integrate code
    [Obsolete("This method is obsolete, please use /feed/latest")]
    [HttpGet("post/latest")]
    public async Task<IActionResult> GetPostLatestComment([FromQuery] Guid postId)
    {
        var result = await _commentService.GetLatestPostCommentInAsync(postId);
        return Ok(result);
    }

    [Obsolete("This method is obsolete, please use /sub-feed/latest")]
    [HttpGet("sub-post/latest")]
    public async Task<IActionResult> GetSubPostLatestComment([FromQuery] Guid postId)
    {
        var result = await _commentService.GetLatestSubPostCommentInAsync(postId);
        return Ok(result);
    }

    [Obsolete("This method is obsolete, please use /feed")]
    [HttpGet("post")]
    public async Task<IActionResult> GetPostComments([FromQuery] CommentLoadR request)
    {
        var result = await _commentService.GetCommentsOfPostAsync(request);
        return Ok(result);
    }

    [Obsolete("This method is obsolete, please use /sub-feed")]
    [HttpGet("sub-post")]
    public async Task<IActionResult> GetSubPostComments([FromQuery] CommentLoadR request)
    {
        var result = await _commentService.GetCommentsOfSubPostAsync(request, PostType.Feed);
        return Ok(result);
    }
    #endregion

    [HttpGet("feed/latest")]
    public async Task<IActionResult> GetFeedLatestComment([FromQuery] Guid postId)
    {
        var result = await _commentService.GetLatestPostCommentInAsync(postId);
        return Ok(result);
    }

    [HttpGet("sub-feed/latest")]
    public async Task<IActionResult> GetSubFeedLatestComment([FromQuery] Guid postId)
    {
        var result = await _commentService.GetLatestSubPostCommentInAsync(postId);
        return Ok(result);
    }

    [HttpGet("feed")]
    public async Task<IActionResult> GetFeedComments([FromQuery] CommentLoadR request)
    {
        var result = await _commentService.GetCommentsOfPostAsync(request);
        return Ok(result);
    }

    [HttpGet("get-comment-by-id/{commentId}")]
    public async Task<IActionResult> GetPostCommentReaction(Guid commentId, [FromQuery] bool isSubPost, [FromQuery] Guid? replyCommentId)
    {
        var req = new BaseR(HttpContext);
        var result = await _commentService.GetCommentById(commentId, isSubPost, req.UserId, replyCommentId);
        return Ok(result);
    }

    [HttpGet("sub-feed")]
    public async Task<IActionResult> GetSubFeedComments([FromQuery] CommentLoadR request)
    {
        var result = await _commentService.GetCommentsOfSubPostAsync(request, PostType.Feed);
        return Ok(result);
    }

    [HttpGet("document")]
    public async Task<IActionResult> GetDocumentComments([FromQuery] CommentLoadR request)
    {
        var result = await _commentService.GetCommentsOfPostAsync(request);
        return Ok(result);
    }

    [HttpGet("document-chapter")]
    public async Task<IActionResult> GetDocumentChapterComments([FromQuery] CommentLoadR request)
    {
        var result = await _commentService.GetCommentsOfSubPostAsync(request, PostType.Document);
        return Ok(result);
    }

    [HttpGet("story")]
    public async Task<IActionResult> GetStoryComments([FromQuery] CommentLoadR request)
    {
        var result = await _commentService.GetCommentsOfPostAsync(request);
        return Ok(result);
    }

    [HttpGet("story-chapter")]
    public async Task<IActionResult> GetStoryChapterComments([FromQuery] CommentLoadR request)
    {
        var result = await _commentService.GetCommentsOfSubPostAsync(request, PostType.Story);
        return Ok(result);
    }

    [HttpGet("comment-most-reaction")]
    public async Task<IActionResult> GetCommentWithMostReaction([FromQuery] CommentMostReactionR request)
    {
        request.Analyze(HttpContext);
        var result = await _commentService.GetCommentWithMostReaction(request);
        return Ok(result);
    }

    [HttpGet("reply-by-comment")]
    public async Task<IActionResult> GetReplyByCommentId([FromQuery] CommentReplyByCommentR input)
    {
        input.Analyze(HttpContext);
        var result = await _commentService.GetReplyByCommentId(input);
        return Ok(result);
    }

    [HttpGet("{id}/reactions")]
    public async Task<IActionResult> GetPostCommentReaction(Guid id, [FromQuery] FeedReactionByTargetR request)
    {
        var result = await _postCommentReactService.GetReactionsByTargetAsync(id, request);
        return Ok(result);
    }

    [HttpGet("{id}/reactions-sub-post")]
    public async Task<IActionResult> GetSubPostCommentReaction(Guid id, [FromQuery] FeedReactionByTargetR request)
    {
        var result = await _subPostCommentReactService.GetReactionsByTargetAsync(id, request);
        return Ok(result);
    }

    #endregion

    #region -- Fields --

    private readonly ICommentService _commentService;
    private readonly IPostCommentReactService _postCommentReactService;
    private readonly ISubPostCommentReactService _subPostCommentReactService;

    #endregion
}
