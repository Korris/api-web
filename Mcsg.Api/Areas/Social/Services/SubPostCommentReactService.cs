namespace Mcsg.Api.Areas.Social.Services;
using Common.Domain.Entities;
using Common.SeedWork.Responses;
using Mcsg.Api.Areas.Social.Interfaces;
using Mcsg.Api.Areas.Social.Models;
using Mcsg.Api.Areas.Social.Requests;

public partial class SubPostCommentReactService : ISubPostCommentReactService
{
    private readonly IReactService<SocialSubPostCommentReaction> _reactService;

    public SubPostCommentReactService(IReactService<SocialSubPostCommentReaction> reactService)
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
