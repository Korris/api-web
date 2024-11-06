using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mcsg.Story.Api.Controllers;

using Interfaces;
using Requests;

[ApiController]
[Route("[controller]")]
public class ReactionController : ControllerBase
{
    #region -- Methods --

    public ReactionController(IPostReactService postReactService, ISubPostReactService subPostReactService, ISubPostCommentReactService subPostCommentReactService, IPostCommentReactService postCommentReactService)
    {
        _postReactService = postReactService;
        _subPostReactService = subPostReactService;
        _subPostCommentReactService = subPostCommentReactService;
        _postCommentReactService = postCommentReactService;
    }

    [HttpGet("post/{postId}")]
    public async Task<IActionResult> GetPostReacts(Guid postId)
    {
        var req = new ReactionReactR(HttpContext) { TargetId = postId };
        var result = await _postReactService.GetReactions(req);
        return Ok(result);
    }

    [HttpPost("post")]
    [Authorize]
    public async Task<IActionResult> AddPostReact(ReactionReactR request)
    {
        request.Analyze(HttpContext);
        var result = await _postReactService.AddReactionToPost(request);
        return Ok(result);
    }

    [HttpDelete("post/{postId}")]
    [Authorize]
    public async Task<IActionResult> DeleteReact(Guid postId)
    {
        var req = new ReactionReactR(HttpContext) { TargetId = postId };
        var result = await _postReactService.RemoveReactionToPost(req);
        return Ok(result);
    }

    [HttpGet("sub-post/{subPostId}")]
    public async Task<IActionResult> GetSubPostReacts(Guid subPostId)
    {
        var req = new ReactionReactR(HttpContext) { TargetId = subPostId };
        var result = await _subPostReactService.GetReactions(req);
        return Ok(result);
    }

    [HttpPost("sub-post")]
    [Authorize]
    public async Task<IActionResult> AddSubPostReact(ReactionReactR request)
    {
        request.Analyze(HttpContext);
        var result = await _subPostReactService.AddReactionToSubPost(request);
        return Ok(result);
    }

    [HttpDelete("sub-post/{subPostId}")]
    [Authorize]
    public async Task<IActionResult> DeleteSubPostReact(Guid subPostId)
    {
        var req = new ReactionReactR(HttpContext) { TargetId = subPostId };
        var result = await _subPostReactService.RemoveReactionToSubPost(req);
        return Ok(result);
    }

    [HttpGet("comment-post/{commentPostId}")]
    public async Task<IActionResult> GetCommentPostReacts(Guid commentPostId)
    {
        var req = new ReactionReactR(HttpContext) { TargetId = commentPostId };
        var result = await _postCommentReactService.GetReactions(req);
        return Ok(result);
    }

    [HttpPost("comment-post")]
    [Authorize]
    public async Task<IActionResult> AddCommentPostReact(ReactionReactR request)
    {
        request.Analyze(HttpContext);
        var result = await _postCommentReactService.AddReaction(request);
        return Ok(result);
    }

    [HttpDelete("comment-post/{commentPostId}")]
    [Authorize]
    public async Task<IActionResult> DeleteCommentPostReact(Guid commentPostId)
    {
        var req = new ReactionReactR(HttpContext) { TargetId = commentPostId };
        var result = await _postCommentReactService.RemoveReaction(req);
        return Ok(result);
    }

    [HttpGet("comment-subpost/{commentSubPostId}")]
    public async Task<IActionResult> GetCommentSubPostReacts(Guid commentSubPostId)
    {
        var req = new ReactionReactR(HttpContext) { TargetId = commentSubPostId };
        var result = await _subPostCommentReactService.GetReactions(req);
        return Ok(result);
    }

    [HttpPost("comment-subpost")]
    [Authorize]
    public async Task<IActionResult> AddCommentSubPostReact(ReactionReactR request)
    {
        var result = await _subPostCommentReactService.AddReaction(request);
        return Ok(result);
    }

    [HttpDelete("comment-subpost/{commentSubPostId}")]
    [Authorize]
    public async Task<IActionResult> DeleteCommentSubPostReact(Guid commentSubPostId)
    {
        var req = new ReactionReactR(HttpContext) { TargetId = commentSubPostId };
        var result = await _subPostCommentReactService.RemoveReaction(req);
        return Ok(result);
    }

    #endregion

    #region -- Fields --

    private readonly IPostReactService _postReactService;

    private readonly ISubPostReactService _subPostReactService;

    private readonly ISubPostCommentReactService _subPostCommentReactService;

    private readonly IPostCommentReactService _postCommentReactService;

    #endregion
}
