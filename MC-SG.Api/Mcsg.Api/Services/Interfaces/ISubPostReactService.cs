using Mcsg.Api.Models;
using Mcsg.Lib.Data.Enums;

namespace Mcsg.Api.Services.Interfaces
{
    public interface ISubPostReactService
    {
        Task<bool> AddReactionToSubPost(Guid postId, ReactionType type);
        Task<bool> RemoveReactionToSubPost(Guid postId);
        Task<ReactionsResponse> GetReactions(Guid postId);
    }
}
