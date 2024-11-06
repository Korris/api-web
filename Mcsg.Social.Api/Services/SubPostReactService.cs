namespace Mcsg.Social.Api.Services;

using Common.Domain.Entities;
using Common.SeedWork.Responses;
using Interfaces;
using Models;
using Requests;

public partial class SubPostReactService : ISubPostReactService
{
    private readonly IReactService<SocialSubPostReaction> _reactService;

    public SubPostReactService(IReactService<SocialSubPostReaction> reactService)
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

    public async Task<PagedResponse<ReactionsUserModel>> GetReactionsByTargetAsync(Guid targetId, FeedReactionByTargetR request)
    {
        return await _reactService.GetReactionsByTargetAsync(targetId, request);
    }
}
