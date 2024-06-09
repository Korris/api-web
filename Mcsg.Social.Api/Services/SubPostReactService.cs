using Mcsg.Social.Api.Models;
using Mcsg.Social.Api.Services.Interfaces;
using Mcsg.Lib.Data.Domain.Entities;
using Mcsg.Lib.Data.Enums;

namespace Mcsg.Social.Api.Services
{
    public partial class SubPostReactService : ISubPostReactService
    {
        private readonly IReactService<SubPostReaction> _reactService;
        public SubPostReactService(IReactService<SubPostReaction> reactService)
        {
            _reactService = reactService;
        }
        public async Task<bool> AddReactionToSubPost(Guid postId, ReactionType type)
        {
            return await _reactService.AddReaction(postId, type);
        }

        public async Task<ReactionsResponse> GetReactions(Guid postId)
        {
            return await _reactService.GetReactions(postId);
        }
        public async Task<bool> RemoveReactionToSubPost(Guid postId)
        {
            return await _reactService.RemoveReaction(postId);
        }
    }
}
