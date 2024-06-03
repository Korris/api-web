namespace Mcsg.Realtime.Api.DTOs
{
    public class UpdateCommentReq : PostCommentReq
    {
        public Guid CommentId { get; set; }
    }
}
