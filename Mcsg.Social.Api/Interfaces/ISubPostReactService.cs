namespace Mcsg.Social.Api.Interfaces;

using Common.Core.Enums;
using Common.SeedWork.Responses;
using Models;
using Requests;

public interface ISubPostReactService
{
    Task<bool> AddReactionToSubPost(Guid postId, ReactionType type);
    Task<bool> RemoveReactionToSubPost(Guid postId);
    Task<ReactionsResponse> GetReactions(Guid postId);
    Task<PagedResponse<ReactionsUserModel>> GetReactionsByTargetAsync(Guid targetId, FeedReactionByTargetR request);
}
