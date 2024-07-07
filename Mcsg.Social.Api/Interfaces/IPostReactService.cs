namespace Mcsg.Social.Api.Interfaces;

using Lib.Data.Entities.Common;
using Lib.Data.Enums;
using Models;
using Requests;

public interface IPostReactService
{
    Task<bool> AddReactionToPost(Guid postId, ReactionType type);
    Task<bool> RemoveReactionToPost(Guid postId);
    Task<ReactionsResponse> GetReactions(Guid postId);
    Task<PagedResults<ReactionsUserModel>> GetReactionsByTargetAsync(Guid targetId, FeedReactionByTargetR request);
}
