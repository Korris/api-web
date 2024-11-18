namespace Mcsg.Story.Api.Interfaces;

using Common.SeedWork.Responses;
using Models;
using Requests;

public interface IPostReactService
{
    Task<ReactionUpdateResponse> AddReactionToPost(ReactionReactR request);
    Task<ReactionUpdateResponse> RemoveReactionToPost(ReactionReactR request);
    Task<ReactionsResponse> GetReactions(ReactionReactR request);
    Task<PagedResponse<ReactionsUserModel>> GetReactionsByTargetAsync(Guid targetId, FeedReactionByTargetR request);
}
