namespace Mcsg.Api.Areas.TapShow.Interfaces;

using Mcsg.Api.Areas.TapShow.Models;
using Mcsg.Api.Areas.TapShow.Requests;

/// <summary>
/// Reactions on TapShow posts and post comments (one reaction per user per target)
/// </summary>
public interface ITapShowReactService
{
    Task<ReactionSummaryResponse> ReactToPostAsync(ReactionReactR request);
    Task<ReactionSummaryResponse> RemovePostReactionAsync(ReactionReactR request);
    Task<ReactionSummaryResponse> ReactToCommentAsync(ReactionReactR request);
    Task<ReactionSummaryResponse> RemoveCommentReactionAsync(ReactionReactR request);
    Task<Dictionary<Guid, ReactionSummaryResponse>> GetPostSummariesAsync(IEnumerable<Guid> postIds, Guid? userId);
    Task<Dictionary<Guid, ReactionSummaryResponse>> GetCommentSummariesAsync(IEnumerable<Guid> commentIds, Guid? userId);
}
