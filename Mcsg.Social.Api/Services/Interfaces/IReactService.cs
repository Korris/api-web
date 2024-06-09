using Mcsg.Social.Api.DTOs;
using Mcsg.Social.Api.Models;
using Mcsg.Lib.Data.Domain.Entities.Common;
using Mcsg.Lib.Data.Entities.Common;
using Mcsg.Lib.Data.Enums;

namespace Mcsg.Social.Api.Services.Interfaces
{
    public interface IReactService<T> where T : ReactionBase, new()
    {
        Task<bool> AddReaction(Guid targetId, ReactionType type);
        Task<bool> RemoveReaction(Guid targetId);
        Task<T> GetReaction(Guid targetId, ReactionType type);
        Task<ReactionsResponse> GetReactions(Guid targetId);
        Task<PagedResults<ReactionsUserModel>> GetReactionsByTargetAsync(Guid targetId, ReactionByTargetRequest request);
    }
}
