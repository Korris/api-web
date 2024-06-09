using Mcsg.Social.Api.DTOs;
using Mcsg.Social.Api.Models;
using Mcsg.Lib.Data.Entities.Common;
using Mcsg.Lib.Data.Enums;

namespace Mcsg.Social.Api.Services.Interfaces
{
    public interface IPostReactService
    {
        Task<bool> AddReactionToPost(Guid postId, ReactionType type);
        Task<bool> RemoveReactionToPost(Guid postId);
        Task<ReactionsResponse> GetReactions(Guid postId);
        Task<PagedResults<ReactionsUserModel>> GetReactionsByTargetAsync(Guid targetId, ReactionByTargetRequest request);
    }
}
