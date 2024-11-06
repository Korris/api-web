namespace Mcsg.Comic.Api.Interfaces;

using Common.SeedWork.Responses;
using Models;
using Requests;

public interface IPostReactService
{
    Task<bool> AddReactionToPost(ReactionReactR request);
    Task<bool> RemoveReactionToPost(ReactionReactR request);
    Task<ReactionsResponse> GetReactions(ReactionReactR request);
    Task<PagedResponse<ReactionsUserModel>> GetReactionsByTargetAsync(Guid targetId, FeedReactionByTargetR request);
}
