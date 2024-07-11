namespace Mcsg.Social.Api.Interfaces;

using Common.SeedWork.Responses;
using Lib.Data.Enums;
using Models;
using Requests;

public interface ISubPostCommentReactService
{
    Task<bool> AddReaction(Guid commentSubPostId, ReactionType type);
    Task<bool> RemoveReaction(Guid commentSubPostId);
    Task<ReactionsResponse> GetReactions(Guid commentSubPostId);
    Task<PagedResponse<ReactionsUserModel>> GetReactionsByTargetAsync(Guid targetId, FeedReactionByTargetR request);
}
