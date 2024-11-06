namespace Mcsg.Story.Api.Services;

using Common.Domain.Entities;
using Interfaces;
using Models;
using Requests;

public partial class SubPostReactService : ISubPostReactService
{
    private readonly IReactService<StorySubPostReaction> _reactService;

    public SubPostReactService(IReactService<StorySubPostReaction> reactService)
    {
        _reactService = reactService;
    }

    public async Task<bool> AddReactionToSubPost(ReactionReactR request)
    {
        return await _reactService.AddReaction(request);
    }

    public async Task<ReactionsResponse> GetReactions(ReactionReactR request)
    {
        return await _reactService.GetReactions(request);
    }

    public async Task<bool> RemoveReactionToSubPost(ReactionReactR request)
    {
        return await _reactService.RemoveReaction(request);
    }
}
