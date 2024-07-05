namespace Mcsg.Social.Api.Interfaces
{
    using Common.Core.Enums;
    using Lib.Data.Entities.Common;
    using Models;
    using Requests;

    public interface ICommentService
    {
        Task<PagedResults<CommentResponse>> GetLatestPostCommentInAsync(Guid postId);
        Task<PagedResults<CommentResponse>> GetLatestSubPostCommentInAsync(Guid postId);
        Task<CommentPagedResults<CommentResponse>> GetCommentsOfPostAsync(CommentLoadReq request);
        Task<CommentPagedResults<CommentResponse>> GetCommentsOfSubPostAsync(CommentLoadReq request, PostType postType);
        Task<CommentPagedResults<MostReactionCommentResponse>> GetCommentWithMostReaction(CommentMostReactionR input);
        Task<List<BasicCommentResponse>> GetReplyByCommentId(CommentReplyByCommentR input);
    }
}
