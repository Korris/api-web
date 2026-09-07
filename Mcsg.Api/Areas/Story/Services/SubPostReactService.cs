namespace Mcsg.Api.Areas.Story.Services;

using Common.Domain.Entities;
using Mcsg.Api.Areas.Story.Interfaces;
using Mcsg.Api.Areas.Story.Models;
using Mcsg.Api.Areas.Story.Requests;

public partial class SubPostReactService : ISubPostReactService
{
    private readonly IReactService<StorySubPostReaction> _reactService;

    public SubPostReactService(IReactService<StorySubPostReaction> reactService)
    {
        _reactService = reactService;
    }

    public async Task<ReactionUpdateResponse> AddReactionToSubPost(ReactionReactR request)
    {
        return await _reactService.AddReaction(request);
    }

    public async Task<ReactionsResponse> GetReactions(ReactionReactR request)
    {
        return await _reactService.GetReactions(request);
    }

    public async Task<ReactionUpdateResponse> RemoveReactionToSubPost(ReactionReactR request)
    {
        return await _reactService.RemoveReaction(request);
    }
}
