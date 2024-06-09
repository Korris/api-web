using Mcsg.Social.Api.Models;
using Mcsg.Lib.Data.Enums;

namespace Mcsg.Social.Api.Services.Interfaces
{
    public interface ISubPostReactService
    {
        Task<bool> AddReactionToSubPost(Guid postId, ReactionType type);
        Task<bool> RemoveReactionToSubPost(Guid postId);
        Task<ReactionsResponse> GetReactions(Guid postId);
    }
}
