namespace Mcsg.Api.Areas.Realtime.Interfaces;

using Mcsg.Api.Areas.Realtime.Requests;

public interface ISocialReplyService
{
    Task<ReplyCommentResp> ReplyComment(ReplyCommentReq req);
    Task<ReplyCommentResp> UpdateReplyComment(UpdateReplyCommentReq req);
    Task<ReplyCommentResp> DeleteReplyComment(DeleteReplyCommentReq req);
}
