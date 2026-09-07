namespace Mcsg.Api.Areas.Comic.Interfaces;

using Mcsg.Api.Areas.Comic.Models;
using Mcsg.Api.Areas.Comic.Requests;

public interface ISubPostReactService
{
    Task<ReactionUpdateResponse> AddReactionToSubPost(ReactionReactR request);
    Task<ReactionUpdateResponse> RemoveReactionToSubPost(ReactionReactR request);
    Task<ReactionsResponse> GetReactions(ReactionReactR request);
}
