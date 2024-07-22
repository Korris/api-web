namespace Mcsg.Social.Api.Interfaces;

using Common.Core.Enums;
using Common.Domain.Entities.Common;
using Common.SeedWork.Responses;
using Models;
using Requests;

public interface IReactService<T> where T : ReactionBase, new()
{
    Task<bool> AddReaction(Guid targetId, ReactionType type);
    Task<bool> RemoveReaction(Guid targetId);
    Task<T> GetReaction(Guid targetId, ReactionType type);
    Task<ReactionsResponse> GetReactions(Guid targetId);
    Task<PagedResponse<ReactionsUserModel>> GetReactionsByTargetAsync(Guid targetId, FeedReactionByTargetR request);
}
