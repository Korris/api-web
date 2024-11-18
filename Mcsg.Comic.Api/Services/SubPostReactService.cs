namespace Mcsg.Comic.Api.Services;

using Common.Domain.Entities;
using Interfaces;
using Models;
using Requests;

public partial class SubPostReactService : ISubPostReactService
{
    private readonly IReactService<ComicSubPostReaction> _reactService;

    public SubPostReactService(IReactService<ComicSubPostReaction> reactService)
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
