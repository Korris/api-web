namespace Mcsg.Social.Api.Interfaces;

using Lib.Data.Entities.Common;
using Lib.Data.Enums;
using Models;
using Requests;

public interface ISubPostCommentReactService
{
    Task<bool> AddReaction(Guid commentSubPostId, ReactionType type);
    Task<bool> RemoveReaction(Guid commentSubPostId);
    Task<ReactionsResponse> GetReactions(Guid commentSubPostId);
    Task<PagedResults<ReactionsUserModel>> GetReactionsByTargetAsync(Guid targetId, FeedReactionByTargetR request);
}
