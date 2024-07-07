namespace Mcsg.Social.Api.Interfaces;

using Lib.Data.Enums;
using Models;

public interface IPostCommentReactService
{
    Task<bool> AddReaction(Guid commentPostId, ReactionType type);
    Task<bool> RemoveReaction(Guid commentPostId);
    Task<ReactionsResponse> GetReactions(Guid commentPostId);
}
