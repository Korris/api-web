using Mcsg.Lib.Data.Enums;

namespace Mcsg.Social.Api.Interfaces
{
    using Mcsg.Lib.Data.Entities.Common;
    using Mcsg.Social.Api.Requests;
    using Models;

    public interface IPostCommentReactService
    {
        Task<bool> AddReaction(Guid commentPostId, ReactionType type);
        Task<bool> RemoveReaction(Guid commentPostId);
        Task<ReactionsResponse> GetReactions(Guid commentPostId);
        Task<PagedResults<ReactionsUserModel>> GetReactionsByTargetAsync(Guid targetId, FeedReactionByTargetR request);
    }
}
