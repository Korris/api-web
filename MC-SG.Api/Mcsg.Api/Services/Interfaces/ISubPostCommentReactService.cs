using Mcsg.Api.Models;
using Mcsg.Lib.Data.Enums;

namespace Mcsg.Api.Services.Interfaces
{
    public interface ISubPostCommentReactService
    {
        Task<bool> AddReaction(Guid commentSubPostId, ReactionType type);
        Task<bool> RemoveReaction(Guid commentSubPostId);
        Task<ReactionsResponse> GetReactions(Guid commentSubPostId);
    }
}
