namespace Mcsg.Story.Api.Interfaces;

using Common.Core.Enums;
using Common.Domain.Entities;
using Common.SeedWork.Responses;
using Models;
using Requests;

public interface IReactService<T> where T : BaseReaction, new()
{
    Task<bool> AddReaction(Guid targetId, ReactionType type, bool isReply = false);
    Task<bool> RemoveReaction(Guid targetId);
    Task<ReactionsResponse> GetReactions(Guid targetId);
    Task<PagedResponse<ReactionsUserModel>> GetReactionsByTargetAsync(Guid targetId, FeedReactionByTargetR request);
}
