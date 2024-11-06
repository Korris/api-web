namespace Mcsg.Document.Api.Interfaces;

using Models;
using Requests;

public interface ISubPostReactService
{
    Task<bool> AddReactionToSubPost(ReactionReactR request);
    Task<bool> RemoveReactionToSubPost(ReactionReactR request);
    Task<ReactionsResponse> GetReactions(ReactionReactR request);
}
