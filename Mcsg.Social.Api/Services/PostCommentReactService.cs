namespace Mcsg.Social.Api.Services;

using Interfaces;
using Lib.Data.Domain.Entities;
using Lib.Data.Enums;
using Mcsg.Lib.Data.Entities.Common;
using Mcsg.Social.Api.Requests;
using Models;

public partial class PostCommentReactService : IPostCommentReactService
{
    private readonly IReactService<PostCommentReaction> _reactService;
    public PostCommentReactService(IReactService<PostCommentReaction> reactService)
    {
        _reactService = reactService;
    }
    public async Task<bool> AddReaction(Guid commentPostId, ReactionType type)
    {
        return await _reactService.AddReaction(commentPostId, type);
    }
    public async Task<PagedResults<ReactionsUserModel>> GetReactionsByTargetAsync(Guid targetId, FeedReactionByTargetR request)
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
