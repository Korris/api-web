namespace Mcsg.Document.Api.Services;
using Common.Domain.Entities;
using Common.SeedWork.Responses;
using Interfaces;
using Models;
using Requests;

public partial class SubPostCommentReactService : ISubPostCommentReactService
{
    private readonly IReactService<DocumentSubPostCommentReaction> _reactService;

    public SubPostCommentReactService(IReactService<DocumentSubPostCommentReaction> reactService)
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
