namespace Mcsg.Api.Areas.Realtime.Requests;

public class UpdateCommentReq : PostCommentReq
{
    public Guid CommentId { get; set; }
}
