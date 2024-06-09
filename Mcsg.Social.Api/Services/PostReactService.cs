using Mcsg.Social.Api.DTOs;
using Mcsg.Social.Api.Models;
using Mcsg.Social.Api.Services.Interfaces;
using Mcsg.Lib.Data.Domain.Entities;
using Mcsg.Lib.Data.Entities.Common;
using Mcsg.Lib.Data.Enums;

namespace Mcsg.Social.Api.Services
{
    public partial class PostReactService : IPostReactService
    {
        private readonly IReactService<PostReaction> _reactService;

        public PostReactService(IReactService<PostReaction> reactService)
        {
            _reactService = reactService;
        }
        public async Task<bool> AddReactionToPost(Guid postId, ReactionType type)
        {
            var reactRes = await _reactService.AddReaction(postId, type);


            return reactRes;
        }

        public async Task<ReactionsResponse> GetReactions(Guid postId)
        {
            return await _reactService.GetReactions(postId);
        }
        public async Task<PagedResults<ReactionsUserModel>> GetReactionsByTargetAsync(Guid targetId, ReactionByTargetRequest request)
        {
            return await _reactService.GetReactionsByTargetAsync(targetId, request);
        }
        public async Task<bool> RemoveReactionToPost(Guid postId)
        {
            return await _reactService.RemoveReaction(postId);
        }
    }
}
