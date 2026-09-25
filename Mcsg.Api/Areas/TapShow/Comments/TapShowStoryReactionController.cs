using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mcsg.Api.Areas.TapShow.Comments;

using Common.Domain.Entities;

/// <summary>
/// TapShow post / post comment reactions with the same routes / requests / responses as the Story ReactionController
/// </summary>
[ApiController]
[Route("api/tapshow/reaction")]
public class TapShowStoryReactionController : ControllerBase
{
    #region -- Methods --

    public TapShowStoryReactionController(ITapShowReactionService<TapShowPostReaction> postReactService, ITapShowReactionService<TapShowPostCommentReaction> postCommentReactService)
    {
        _postReactService = postReactService;
        _postCommentReactService = postCommentReactService;
    }

    [HttpGet("post/{postId}")]
    public async Task<IActionResult> GetPostReacts(Guid postId)
    {
        var req = new ReactionReactR(HttpContext) { TargetId = postId };
        return Ok(await _postReactService.GetReactions(req));
    }

    [HttpPost("post")]
    [Authorize]
    public async Task<IActionResult> AddPostReact(ReactionReactR request)
    {
        request.Analyze(HttpContext);
        return Ok(await _postReactService.AddReaction(request));
    }

    [HttpDelete("post/{postId}")]
    [Authorize]
    public async Task<IActionResult> DeleteReact(Guid postId)
    {
        var req = new ReactionReactR(HttpContext) { TargetId = postId };
        return Ok(await _postReactService.RemoveReaction(req));
    }

    [HttpGet("comment-post/{commentPostId}")]
    public async Task<IActionResult> GetCommentPostReacts(Guid commentPostId)
    {
        var req = new ReactionReactR(HttpContext) { TargetId = commentPostId };
        return Ok(await _postCommentReactService.GetReactions(req));
    }

    [HttpPost("comment-post")]
    [Authorize]
    public async Task<IActionResult> AddCommentPostReact(ReactionReactR request)
    {
        request.Analyze(HttpContext);
        return Ok(await _postCommentReactService.AddReaction(request));
    }

    [HttpDelete("comment-post/{commentPostId}")]
    [Authorize]
    public async Task<IActionResult> DeleteCommentPostReact(Guid commentPostId)
    {
        var req = new ReactionReactR(HttpContext) { TargetId = commentPostId };
        return Ok(await _postCommentReactService.RemoveReaction(req));
    }

    #endregion

    #region -- Fields --

    private readonly ITapShowReactionService<TapShowPostReaction> _postReactService;
    private readonly ITapShowReactionService<TapShowPostCommentReaction> _postCommentReactService;

    #endregion
}
