namespace Mcsg.Story.Api.Interfaces;

using Common.Domain.Entities;
using Common.SeedWork.Responses;
using Models;
using Requests;

public interface IReactService<T> where T : BaseReaction, new()
{
    Task<bool> AddReaction(ReactionReactR request);
    Task<bool> RemoveReaction(ReactionReactR request);
    Task<ReactionsResponse> GetReactions(ReactionReactR request);
    Task<PagedResponse<ReactionsUserModel>> GetReactionsByTargetAsync(Guid targetId, FeedReactionByTargetR request);
}
