namespace Mcsg.Comic.Api.Interfaces;

using Common.Core.Enums;
using Common.SeedWork.Responses;
using Models;
using Requests;

public interface IPostCommentReactService
{
    Task<bool> AddReaction(Guid commentPostId, ReactionType type, bool isReply = false);
    Task<bool> RemoveReaction(Guid commentPostId);
    Task<ReactionsResponse> GetReactions(Guid commentPostId);
    Task<PagedResponse<ReactionsUserModel>> GetReactionsByTargetAsync(Guid targetId, FeedReactionByTargetR request);
}
