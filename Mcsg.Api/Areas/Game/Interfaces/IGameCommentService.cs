namespace Mcsg.Api.Areas.Game.Interfaces;

using Common.SeedWork.Responses;
using Mcsg.Api.Areas.Game.Models;
using Mcsg.Api.Areas.Game.Requests;

/// <summary>
/// Comments (1 level of replies) on game posts
/// </summary>
public interface IGameCommentService
{
    Task<CommentResponse> CreateAsync(CommentCreateR request);
    Task<CommentResponse> UpdateAsync(CommentUpdateR request);
    Task<bool> DeleteAsync(Guid id, Guid? userId);
    Task<PagedResponse<CommentResponse>> ListAsync(CommentListR request);
}
