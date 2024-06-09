using Mcsg.Social.Api.Models;
using Mcsg.Lib.Data.Enums;

namespace Mcsg.Social.Api.Services.Interfaces
{
    public interface IPostCommentReactService
    {
        Task<bool> AddReaction(Guid commentPostId, ReactionType type);
        Task<bool> RemoveReaction(Guid commentPostId);
        Task<ReactionsResponse> GetReactions(Guid commentPostId);
    }
}
