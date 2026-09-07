namespace Mcsg.Api.Areas.Realtime.Interfaces;

using Mcsg.Api.Areas.Realtime.Requests;

public interface ISocialCommentService
{
    Task<PostCommentResp> PostComment(PostCommentReq req);
    Task<PostCommentResp> UpdateComment(UpdateCommentReq req);
    Task<PostCommentResp> DeleteComment(DeleteCommentReq req);
}
