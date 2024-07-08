namespace Mcsg.Social.Api.Interfaces;

using Lib.Data.Entities.Common;
using Lib.Data.Enums;
using Models;
using Requests;

public interface IPostCommentReactService
{
    Task<bool> AddReaction(Guid commentPostId, ReactionType type);
    Task<bool> RemoveReaction(Guid commentPostId);
    Task<ReactionsResponse> GetReactions(Guid commentPostId);
    Task<PagedResults<ReactionsUserModel>> GetReactionsByTargetAsync(Guid targetId, FeedReactionByTargetR request);
}
