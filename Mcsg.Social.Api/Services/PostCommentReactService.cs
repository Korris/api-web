using Mcsg.Social.Api.Models;
using Mcsg.Social.Api.Services.Interfaces;
using Mcsg.Lib.Data.Domain.Entities;
using Mcsg.Lib.Data.Enums;

namespace Mcsg.Social.Api.Services
{
    public partial class PostCommentReactService : IPostCommentReactService
    {
        private readonly IReactService<PostCommentReaction> _reactService;
        public PostCommentReactService(IReactService<PostCommentReaction> reactService)
        {
            _reactService = reactService;
        }
        public async Task<bool> AddReaction(Guid commentPostId, ReactionType type)
        {
            return await _reactService.AddReaction(commentPostId, type);
        }

        public async Task<ReactionsResponse> GetReactions(Guid commentPostId)
        {
            return await _reactService.GetReactions(commentPostId);
        }
        public async Task<bool> RemoveReaction(Guid commentPostId)
        {
            return await _reactService.RemoveReaction(commentPostId);
        }
    }
}
