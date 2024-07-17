namespace Mcsg.Comic.Api.Interfaces;

using Common.SeedWork.Responses;
using Lib.Data.Enums;
using Models;
using Requests;

public interface IPostCommentReactService
{
    Task<bool> AddReaction(Guid commentPostId, ReactionType type);
    Task<bool> RemoveReaction(Guid commentPostId);
    Task<ReactionsResponse> GetReactions(Guid commentPostId);
    Task<PagedResponse<ReactionsUserModel>> GetReactionsByTargetAsync(Guid targetId, FeedReactionByTargetR request);
}
