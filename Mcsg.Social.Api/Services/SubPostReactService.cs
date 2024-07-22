namespace Mcsg.Social.Api.Services;

using Common.Core.Enums;
using Common.Domain.Entities;
using Interfaces;
using Models;

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
