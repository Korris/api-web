namespace Mcsg.Realtime.Api.Requests
{
    public class UpdateCommentReq : PostCommentReq
    {
        public Guid CommentId { get; set; }
    }
}
