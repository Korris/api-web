namespace Mcsg.Api.Areas.Document.Interfaces;

using Mcsg.Api.Areas.Document.Models;
using Mcsg.Api.Areas.Document.Requests;

public interface ISubPostReactService
{
    Task<ReactionUpdateResponse> AddReactionToSubPost(ReactionReactR request);
    Task<ReactionUpdateResponse> RemoveReactionToSubPost(ReactionReactR request);
    Task<ReactionsResponse> GetReactions(ReactionReactR request);
}
