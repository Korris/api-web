using Mcsg.Lib.Data.Enums;

namespace Mcsg.Social.Api.Interfaces
{
    using Models;

    public interface IPostCommentReactService
    {
        Task<bool> AddReaction(Guid commentPostId, ReactionType type);
        Task<bool> RemoveReaction(Guid commentPostId);
        Task<ReactionsResponse> GetReactions(Guid commentPostId);
    }
}
