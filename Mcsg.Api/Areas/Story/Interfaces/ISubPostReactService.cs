namespace Mcsg.Api.Areas.Story.Interfaces;

using Mcsg.Api.Areas.Story.Models;
using Mcsg.Api.Areas.Story.Requests;

public interface ISubPostReactService
{
    Task<ReactionUpdateResponse> AddReactionToSubPost(ReactionReactR request);
    Task<ReactionUpdateResponse> RemoveReactionToSubPost(ReactionReactR request);
    Task<ReactionsResponse> GetReactions(ReactionReactR request);
}
