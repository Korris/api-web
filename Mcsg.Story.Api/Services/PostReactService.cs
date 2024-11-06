namespace Mcsg.Story.Api.Services;

using Common.Domain.Entities;
using Common.SeedWork.Responses;
using Interfaces;
using Models;
using Requests;

public partial class PostReactService : IPostReactService
{
    private readonly IReactService<StoryPostReaction> _reactService;

    public PostReactService(IReactService<StoryPostReaction> reactService)
    {
        _reactService = reactService;
    }

    public async Task<bool> AddReactionToPost(ReactionReactR request)
    {
        return await _reactService.AddReaction(request);
    }

    public async Task<ReactionsResponse> GetReactions(ReactionReactR request)
    {
        return await _reactService.GetReactions(request);
    }

    public async Task<PagedResponse<ReactionsUserModel>> GetReactionsByTargetAsync(Guid targetId, FeedReactionByTargetR request)
    {
        return await _reactService.GetReactionsByTargetAsync(targetId, request);
    }

    public async Task<bool> RemoveReactionToPost(ReactionReactR request)
    {
        return await _reactService.RemoveReaction(request);
    }
}
