namespace Mcsg.Api.Areas.Game.Interfaces;

using Mcsg.Api.Areas.Game.Models;
using Mcsg.Api.Areas.Game.Requests;

/// <summary>
/// Reactions on game posts and game post comments (one reaction per user per target)
/// </summary>
public interface IGameReactService
{
    Task<ReactionSummaryResponse> ReactToPostAsync(ReactionReactR request);
    Task<ReactionSummaryResponse> RemovePostReactionAsync(ReactionReactR request);
    Task<ReactionSummaryResponse> ReactToCommentAsync(ReactionReactR request);
    Task<ReactionSummaryResponse> RemoveCommentReactionAsync(ReactionReactR request);
    Task<Dictionary<Guid, ReactionSummaryResponse>> GetPostSummariesAsync(IEnumerable<Guid> postIds, Guid? userId);
    Task<Dictionary<Guid, ReactionSummaryResponse>> GetCommentSummariesAsync(IEnumerable<Guid> commentIds, Guid? userId);
}
