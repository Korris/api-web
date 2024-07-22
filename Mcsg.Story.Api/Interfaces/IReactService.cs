namespace Mcsg.Story.Api.Interfaces;

using Common.Core.Enums;
using Common.SeedWork.Responses;
using Mcsg.Common.Domain.Entities.Common;
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
