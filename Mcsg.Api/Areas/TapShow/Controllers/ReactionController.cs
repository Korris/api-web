using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mcsg.Api.Areas.TapShow.Controllers;

using Mcsg.Api.Areas.TapShow.Interfaces;
using Mcsg.Api.Areas.TapShow.Models;
using Mcsg.Api.Areas.TapShow.Requests;

/// <summary>
/// Reactions on TapShow posts and comments (api/tapshow/reaction). Same route shape as the Game ReactionController.
/// </summary>
[ApiController]
[Route("api/tapshow/[controller]")]
public class ReactionController : ControllerBase
{
    #region -- Methods --

    public ReactionController(ITapShowReactService reactService)
    {
        _reactService = reactService;
    }

    [HttpGet("comment/{commentId}")]
    public async Task<IActionResult> GetCommentReacts(Guid commentId)
    {
        var request = Analyze(commentId);
        var map = await _reactService.GetCommentSummariesAsync(new[] { commentId }, request.UserId);
        return Ok(map.TryGetValue(commentId, out var summary) ? summary : new ReactionSummaryResponse { TargetId = commentId });
    }

    [HttpPost("comment"), Authorize]
    public async Task<IActionResult> AddCommentReact([FromBody] ReactionReactR request)
    {
        request.Analyze(HttpContext);
        return Ok(await _reactService.ReactToCommentAsync(request));
    }

    [HttpDelete("comment/{commentId}"), Authorize]
    public async Task<IActionResult> DeleteCommentReact(Guid commentId)
    {
        return Ok(await _reactService.RemoveCommentReactionAsync(Analyze(commentId)));
    }

    #endregion

    #region -- Helpers --

    private ReactionReactR Analyze(Guid targetId)
    {
        var request = new ReactionReactR { TargetId = targetId };
        request.Analyze(HttpContext);
        return request;
    }

    #endregion

    #region -- Fields --

    private readonly ITapShowReactService _reactService;

    #endregion
}
