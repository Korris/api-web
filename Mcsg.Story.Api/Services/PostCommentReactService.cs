namespace Mcsg.Story.Api.Services;

using Common.Core.Enums;
using Common.Domain.Entities;
using Common.SeedWork.Responses;
using Interfaces;
using Models;
using Requests;

public partial class PostCommentReactService : IPostCommentReactService
{
    private readonly IReactService<StoryPostCommentReaction> _reactService;
    public PostCommentReactService(IReactService<StoryPostCommentReaction> reactService)
    {
        _reactService = reactService;
    }
    public async Task<bool> AddReaction(Guid commentPostId, ReactionType type)
    {
        return await _reactService.AddReaction(commentPostId, type);
    }
    public async Task<PagedResponse<ReactionsUserModel>> GetReactionsByTargetAsync(Guid targetId, FeedReactionByTargetR request)
    {
        return await _reactService.GetReactionsByTargetAsync(targetId, request);
    }

    public async Task<ReactionsResponse> GetReactions(Guid commentPostId)
    {
        return await _reactService.GetReactions(commentPostId);
    }
    public async Task<bool> RemoveReaction(Guid commentPostId)
    {
        return await _reactService.RemoveReaction(commentPostId);
    }
}
