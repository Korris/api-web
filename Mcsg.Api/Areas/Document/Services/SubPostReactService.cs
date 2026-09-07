namespace Mcsg.Api.Areas.Document.Services;

using Common.Domain.Entities;
using Mcsg.Api.Areas.Document.Interfaces;
using Mcsg.Api.Areas.Document.Models;
using Mcsg.Api.Areas.Document.Requests;

public partial class SubPostReactService : ISubPostReactService
{
    private readonly IReactService<DocumentSubPostReaction> _reactService;

    public SubPostReactService(IReactService<DocumentSubPostReaction> reactService)
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
