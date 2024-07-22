namespace Mcsg.Realtime.Api.Interfaces;

using Requests;

public interface ISocialCommentService
{
    Task<PostCommentResp> PostComment(PostCommentReq req);
    Task<PostCommentResp> UpdateComment(UpdateCommentReq req);
    Task<PostCommentResp> DeleteComment(DeleteCommentReq req);
}
