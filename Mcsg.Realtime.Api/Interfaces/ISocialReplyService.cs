namespace Mcsg.Realtime.Api.Interfaces;

using Requests;

public interface ISocialReplyService
{
    Task<ReplyCommentResp> ReplyComment(ReplyCommentReq req);
    Task<ReplyCommentResp> UpdateReplyComment(UpdateReplyCommentReq req);
    Task<ReplyCommentResp> DeleteReplyComment(DeleteReplyCommentReq req);
}
