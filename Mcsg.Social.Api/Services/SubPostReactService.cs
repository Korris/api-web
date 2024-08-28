namespace Mcsg.Social.Api.Services;

using Api.Requests;
using Common.Core.Enums;
using Common.Domain.Entities;
using Common.SeedWork.Responses;
using Interfaces;
using Models;

public partial class SubPostReactService : ISubPostReactService
{
    private readonly IReactService<SocialSubPostReaction> _reactService;
    public SubPostReactService(IReactService<SocialSubPostReaction> reactService)
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

    public async Task<PagedResponse<ReactionsUserModel>> GetReactionsByTargetAsync(Guid targetId, FeedReactionByTargetR request)
    {
        return await _reactService.GetReactionsByTargetAsync(targetId, request);
    }

    public async Task<bool> RemoveReactionToSubPost(Guid postId)
    {
        return await _reactService.RemoveReaction(postId);
    }
}
