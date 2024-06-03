using Mcsg.Api.Models;
using Mcsg.Api.Services.Interfaces;
using Mcsg.Lib.Data.Domain.Entities;
using Mcsg.Lib.Data.Enums;

namespace Mcsg.Api.Services
{
    public partial class SubPostCommentReactService : ISubPostCommentReactService
    {
        private readonly IReactService<SubPostCommentReaction> _reactService;
        public SubPostCommentReactService(IReactService<SubPostCommentReaction> reactService)
        {
            _reactService = reactService;
        }
        public async Task<bool> AddReaction(Guid commentSubPostId, ReactionType type)
        {
            return await _reactService.AddReaction(commentSubPostId, type);
        }

        public async Task<ReactionsResponse> GetReactions(Guid commentSubPostId)
        {
            return await _reactService.GetReactions(commentSubPostId);
        }
        public async Task<bool> RemoveReaction(Guid commentSubPostId)
        {
            return await _reactService.RemoveReaction(commentSubPostId);
        }
    }
}
