namespace Mcsg.Comic.Api.Interfaces;

using Common.SeedWork.Responses;
using Lib.Data.Enums;
using Models;
using Requests;

public interface IPostReactService
{
    Task<bool> AddReactionToPost(Guid postId, ReactionType type);
    Task<bool> RemoveReactionToPost(Guid postId);
    Task<ReactionsResponse> GetReactions(Guid postId);
    Task<PagedResponse<ReactionsUserModel>> GetReactionsByTargetAsync(Guid targetId, FeedReactionByTargetR request);
}
