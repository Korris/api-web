namespace Mcsg.Document.Api.Services;

using Common.Core.Enums;
using Common.Domain.Entities;
using Common.SeedWork.Responses;
using Interfaces;
using Models;
using Requests;

public partial class PostReactService : IPostReactService
{
    private readonly IReactService<DocumentPostReaction> _reactService;

    public PostReactService(IReactService<DocumentPostReaction> reactService)
    {
        _reactService = reactService;
    }
    public async Task<bool> AddReactionToPost(Guid postId, ReactionType type)
    {
        var reactRes = await _reactService.AddReaction(postId, type);


        return reactRes;
    }

    public async Task<ReactionsResponse> GetReactions(Guid postId)
    {
        return await _reactService.GetReactions(postId);
    }
    public async Task<PagedResponse<ReactionsUserModel>> GetReactionsByTargetAsync(Guid targetId, FeedReactionByTargetR request)
    {
        return await _reactService.GetReactionsByTargetAsync(targetId, request);
    }
    public async Task<bool> RemoveReactionToPost(Guid postId)
    {
        return await _reactService.RemoveReaction(postId);
    }
}
