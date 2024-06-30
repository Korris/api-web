namespace Mcsg.Social.Api.Interfaces
{
    using Common.Core.Enums;
    using DTOs;
    using Lib.Data.Entities.Common;
    using Models;

    public interface ICommentService
    {
        Task<PagedResults<CommentResponse>> GetLatestPostCommentInAsync(Guid postId);
        Task<PagedResults<CommentResponse>> GetLatestSubPostCommentInAsync(Guid postId);
        Task<CommentPagedResults<CommentResponse>> GetCommentsOfPostAsync(CommentLoadReq request);
        Task<CommentPagedResults<CommentResponse>> GetCommentsOfSubPostAsync(CommentLoadReq request, PostType postType);
        Task<CommentPagedResults<MostReactionCommentResponse>> GetCommentWithMostReaction(MostReactionCommentInput input);
        Task<List<BasicCommentResponse>> GetReplyByCommentId(ReplyByCommentInput input);
    }
}
