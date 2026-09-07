namespace Mcsg.Api.Areas.Story.Services;
using Common.Domain.Entities;
using Common.SeedWork.Responses;
using Mcsg.Api.Areas.Story.Interfaces;
using Mcsg.Api.Areas.Story.Models;
using Mcsg.Api.Areas.Story.Requests;

public partial class SubPostCommentReactService : ISubPostCommentReactService
{
    private readonly IReactService<StorySubPostCommentReaction> _reactService;

    public SubPostCommentReactService(IReactService<StorySubPostCommentReaction> reactService)
    {
        _reactService = reactService;
    }

    public async Task<ReactionUpdateResponse> AddReaction(ReactionReactR request)
    {
        return await _reactService.AddReaction(request);
    }

    public async Task<PagedResponse<ReactionsUserModel>> GetReactionsByTargetAsync(Guid targetId, FeedReactionByTargetR request)
    {
        return await _reactService.GetReactionsByTargetAsync(targetId, request);
    }

    public async Task<ReactionsResponse> GetReactions(ReactionReactR request)
    {
        return await _reactService.GetReactions(request);
    }

    public async Task<ReactionUpdateResponse> RemoveReaction(ReactionReactR request)
    {
        return await _reactService.RemoveReaction(request);
    }
}
