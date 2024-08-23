namespace Mcsg.Comic.Api.Interfaces;

using Common.Core.Enums;
using Common.SeedWork.Responses;
using Models;
using Requests;

public interface ICommentService
{
    Task<PagedResponse<CommentResponse>> GetLatestPostCommentInAsync(Guid postId);
    Task<PagedResponse<CommentResponse>> GetLatestSubPostCommentInAsync(Guid postId);
    Task<CommentPagedResults<CommentResponse>> GetCommentsOfPostAsync(CommentLoadR request);
    Task<CommentPagedResults<CommentResponse>> GetCommentsOfSubPostAsync(CommentLoadR request, PostType postType);
    Task<CommentPagedResults<MostReactionCommentResponse>> GetCommentWithMostReaction(CommentMostReactionR input);
    Task<PagedResponse<BasicCommentResponse>> GetReplyByCommentId(CommentReplyByCommentR input);
    Task<CommentResponse> GetCommentById(Guid commentId, bool isSubPost);
}
