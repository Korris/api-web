namespace Mcsg.Social.Api.Interfaces;

using Lib.Data.Enums;
using Models;

public interface ISubPostCommentReactService
{
    Task<bool> AddReaction(Guid commentSubPostId, ReactionType type);
    Task<bool> RemoveReaction(Guid commentSubPostId);
    Task<ReactionsResponse> GetReactions(Guid commentSubPostId);
}
