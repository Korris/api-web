namespace Mcsg.Api.Areas.Comic.Interfaces;

using Common.SeedWork.Responses;
using Mcsg.Api.Areas.Comic.Models;
using Mcsg.Api.Areas.Comic.Requests;

public interface IPostReactService
{
    Task<ReactionUpdateResponse> AddReactionToPost(ReactionReactR request);
    Task<ReactionUpdateResponse> RemoveReactionToPost(ReactionReactR request);
    Task<ReactionsResponse> GetReactions(ReactionReactR request);
    Task<PagedResponse<ReactionsUserModel>> GetReactionsByTargetAsync(Guid targetId, FeedReactionByTargetR request);
}
