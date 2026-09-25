namespace Mcsg.Api.Areas.TapShow.Interfaces;

using Common.SeedWork.Responses;
using Mcsg.Api.Areas.TapShow.Models;
using Mcsg.Api.Areas.TapShow.Requests;

/// <summary>
/// Comments (1 level of replies) on TapShow posts
/// </summary>
public interface ITapShowCommentService
{
    Task<CommentResponse> CreateAsync(CommentCreateR request);
    Task<CommentResponse> UpdateAsync(CommentUpdateR request);
    Task<bool> DeleteAsync(Guid id, Guid? userId);
    Task<PagedResponse<CommentResponse>> ListAsync(CommentListR request);
    Task<bool> CheckPostExistedAsync(CommentCheckPostExistedR request);
    Task<Dictionary<Guid, List<MostReactionCommentResponse>>> GetTopByPostIdsAsync(IReadOnlyCollection<Guid> postIds, Guid? currentUserId, int take);
}
