namespace Mcsg.Document.Api.Services;

using Common.Core.Enums;
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
    public async Task<bool> AddReaction(Guid commentSubPostId, ReactionType type, bool isReply = false)
    {
        return await _reactService.AddReaction(commentSubPostId, type, isReply);
    }
    public async Task<PagedResponse<ReactionsUserModel>> GetReactionsByTargetAsync(Guid targetId, FeedReactionByTargetR request)
    {
        return await _reactService.GetReactionsByTargetAsync(targetId, request);
    }
    public async Task<ReactionsResponse> GetReactions(Guid commentSubPostId)
    {
        return await _reactService.GetReactions(commentSubPostId);
    }
    public async Task<bool> RemoveReaction(Guid commentSubPostId)
    {
        return await _reactService.RemoveReaction(commentSubPostId);
    }
}
