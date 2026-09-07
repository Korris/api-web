namespace Mcsg.Api.Areas.Comic.Services;

using Common.Domain.Entities;
using Common.SeedWork.Responses;
using Mcsg.Api.Areas.Comic.Interfaces;
using Mcsg.Api.Interfaces;
using Mcsg.Api.Areas.Comic.Models;
using Mcsg.Api.Areas.Comic.Requests;

public partial class PostCommentReactService : IPostCommentReactService
{
    private readonly IReactService<ComicPostCommentReaction> _reactService;

    public PostCommentReactService(IReactService<ComicPostCommentReaction> reactService)
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
