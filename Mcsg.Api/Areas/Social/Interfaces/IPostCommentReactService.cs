namespace Mcsg.Api.Areas.Social.Interfaces;

using Common.SeedWork.Responses;
using Mcsg.Api.Areas.Social.Models;
using Mcsg.Api.Areas.Social.Requests;

public interface IPostCommentReactService
{
    Task<ReactionUpdateResponse> AddReaction(ReactionReactR request);
    Task<ReactionUpdateResponse> RemoveReaction(ReactionReactR request);
    Task<ReactionsResponse> GetReactions(ReactionReactR request);
    Task<PagedResponse<ReactionsUserModel>> GetReactionsByTargetAsync(Guid targetId, FeedReactionByTargetR request);
}
