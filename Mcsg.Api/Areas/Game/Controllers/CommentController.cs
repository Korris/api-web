using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mcsg.Api.Areas.Game.Controllers;

using Mcsg.Api.Areas.Game.Interfaces;
using Mcsg.Api.Areas.Game.Requests;

/// <summary>
/// Comments on game posts (api/game/comment). REST replacement for the Comic SignalR comment flow.
/// </summary>
[ApiController]
[Route("api/game/[controller]")]
public class CommentController : ControllerBase
{
    #region -- Methods --

    public CommentController(IGameCommentService commentService)
    {
        _commentService = commentService;
    }

    /// <summary>
    /// Root comments of a post, newest first
    /// </summary>
    [HttpGet("post/{hashId}")]
    public async Task<IActionResult> ListByPost(string hashId, [FromQuery] CommentListR request)
    {
        request.Analyze(HttpContext);
        request.PostHashId = hashId;
        request.ParentId = null;
        return Ok(await _commentService.ListAsync(request));
    }

    /// <summary>
    /// Replies of a root comment, oldest first
    /// </summary>
    [HttpGet("{id}/replies")]
    public async Task<IActionResult> ListReplies(Guid id, [FromQuery] CommentListR request)
    {
        request.Analyze(HttpContext);
        request.ParentId = id;
        return Ok(await _commentService.ListAsync(request));
    }

    /// <summary>
    /// Create a comment (ParentId null) or a reply (ParentId = root comment id)
    /// </summary>
    [HttpPost, Authorize]
    public async Task<IActionResult> Create([FromBody] CommentCreateR request)
    {
        request.Analyze(HttpContext);
        return Ok(await _commentService.CreateAsync(request));
    }

    [HttpPut("{id}"), Authorize]
    public async Task<IActionResult> Update(Guid id, [FromBody] CommentUpdateR request)
    {
        request.Analyze(HttpContext);
        request.Id = id;
        return Ok(await _commentService.UpdateAsync(request));
    }

    /// <summary>
    /// Allowed for the comment author or the post owner
    /// </summary>
    [HttpDelete("{id}"), Authorize]
    public async Task<IActionResult> Delete(Guid id)
    {
        var request = new CommentUpdateR();
        request.Analyze(HttpContext);
        return Ok(await _commentService.DeleteAsync(id, request.UserId));
    }

    #endregion

    #region -- Fields --

    private readonly IGameCommentService _commentService;

    #endregion
}
