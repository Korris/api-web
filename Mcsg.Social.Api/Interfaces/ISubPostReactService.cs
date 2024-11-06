namespace Mcsg.Social.Api.Interfaces;

using Common.SeedWork.Responses;
using Models;
using Requests;

public interface ISubPostReactService
{
    Task<bool> AddReactionToSubPost(ReactionReactR request);
    Task<bool> RemoveReactionToSubPost(ReactionReactR request);
    Task<ReactionsResponse> GetReactions(ReactionReactR request);
    Task<PagedResponse<ReactionsUserModel>> GetReactionsByTargetAsync(Guid targetId, FeedReactionByTargetR request);
}
