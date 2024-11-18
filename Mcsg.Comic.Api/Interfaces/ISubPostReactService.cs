namespace Mcsg.Comic.Api.Interfaces;

using Models;
using Requests;

public interface ISubPostReactService
{
    Task<ReactionUpdateResponse> AddReactionToSubPost(ReactionReactR request);
    Task<ReactionUpdateResponse> RemoveReactionToSubPost(ReactionReactR request);
    Task<ReactionsResponse> GetReactions(ReactionReactR request);
}
