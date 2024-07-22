namespace Mcsg.Story.Api.Interfaces;

using Common.Core.Enums;
using Common.SeedWork.Responses;
using Models;
using Requests;

public interface ISubPostCommentReactService
{
    Task<bool> AddReaction(Guid commentSubPostId, ReactionType type);
    Task<bool> RemoveReaction(Guid commentSubPostId);
    Task<ReactionsResponse> GetReactions(Guid commentSubPostId);
    Task<PagedResponse<ReactionsUserModel>> GetReactionsByTargetAsync(Guid targetId, FeedReactionByTargetR request);
}
