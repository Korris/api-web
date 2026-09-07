namespace Mcsg.Api.Areas.Social.Interfaces;

using Common.SeedWork.Responses;
using Mcsg.Api.Areas.Social.Models;
using Mcsg.Api.Areas.Social.Requests;

public interface ISubPostReactService
{
    Task<ReactionUpdateResponse> AddReactionToSubPost(ReactionReactR request);
    Task<ReactionUpdateResponse> RemoveReactionToSubPost(ReactionReactR request);
    Task<ReactionsResponse> GetReactions(ReactionReactR request);
    Task<PagedResponse<ReactionsUserModel>> GetReactionsByTargetAsync(Guid targetId, FeedReactionByTargetR request);
}
