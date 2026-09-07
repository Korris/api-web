namespace Mcsg.Api.Areas.Story.Interfaces;

using Common.SeedWork.Responses;
using Mcsg.Api.Areas.Story.Models;
using Mcsg.Api.Areas.Story.Requests;

public interface IPostReactService
{
    Task<ReactionUpdateResponse> AddReactionToPost(ReactionReactR request);
    Task<ReactionUpdateResponse> RemoveReactionToPost(ReactionReactR request);
    Task<ReactionsResponse> GetReactions(ReactionReactR request);
    Task<PagedResponse<ReactionsUserModel>> GetReactionsByTargetAsync(Guid targetId, FeedReactionByTargetR request);
}
