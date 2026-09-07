namespace Mcsg.Api.Areas.Document.Interfaces;

using Common.Domain.Entities;
using Common.SeedWork.Responses;
using Mcsg.Api.Areas.Document.Models;
using Mcsg.Api.Areas.Document.Requests;

public interface IReactService<T> where T : BaseReaction, new()
{
    Task<ReactionUpdateResponse> AddReaction(ReactionReactR request);
    Task<ReactionUpdateResponse> RemoveReaction(ReactionReactR request);
    Task<ReactionsResponse> GetReactions(ReactionReactR request);
    Task<PagedResponse<ReactionsUserModel>> GetReactionsByTargetAsync(Guid targetId, FeedReactionByTargetR request);
}
