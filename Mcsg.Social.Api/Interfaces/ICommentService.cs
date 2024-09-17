namespace Mcsg.Social.Api.Interfaces;

using Common.Core.Enums;
using Common.SeedWork.Responses;
using Models;
using Requests;

public interface ICommentService
{
    Task<PagedResponse<CommentResponse>> GetLatestPostCommentInAsync(Guid postId);
    Task<PagedResponse<CommentResponse>> GetLatestSubPostCommentInAsync(Guid postId);
    Task<CommentPagedResults<CommentResponse>> GetCommentsOfPostAsync(CommentLoadR request, Guid? userId);
    Task<CommentPagedResults<CommentResponse>> GetCommentsOfSubPostAsync(CommentLoadR request, PostType postType, Guid? userId);
    Task<CommentPagedResults<MostReactionCommentResponse>> GetCommentWithMostReaction(CommentMostReactionR request);
    Task<PagedResponse<MostReactionCommentResponse>> GetReplyByCommentId(CommentReplyByCommentR input, Guid? userId);
    Task<CommentResponse> GetCommentById(Guid commentId, bool isSubPost, Guid? userId, Guid? replyCommentId);
}
