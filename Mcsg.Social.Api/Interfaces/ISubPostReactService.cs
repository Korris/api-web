namespace Mcsg.Social.Api.Interfaces;

using Lib.Data.Enums;
using Models;

public interface ISubPostReactService
{
    Task<bool> AddReactionToSubPost(Guid postId, ReactionType type);
    Task<bool> RemoveReactionToSubPost(Guid postId);
    Task<ReactionsResponse> GetReactions(Guid postId);
}
