namespace Mcsg.Social.Api.Interfaces
{
    using DTOs;
    using Lib.Data.Domain.Entities.Common;
    using Lib.Data.Entities.Common;
    using Lib.Data.Enums;
    using Models;

    public interface IReactService<T> where T : ReactionBase, new()
    {
        Task<bool> AddReaction(Guid targetId, ReactionType type);
        Task<bool> RemoveReaction(Guid targetId);
        Task<T> GetReaction(Guid targetId, ReactionType type);
        Task<ReactionsResponse> GetReactions(Guid targetId);
        Task<PagedResults<ReactionsUserModel>> GetReactionsByTargetAsync(Guid targetId, ReactionByTargetRequest request);
    }
}
