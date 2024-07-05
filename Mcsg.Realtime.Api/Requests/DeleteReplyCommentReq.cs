namespace Mcsg.Realtime.Api.DTOs
{
    public class DeleteReplyCommentReq
    {
        public Guid ReplyCommentId { get; set; }
        public Guid ReplyToCommentId { get; set; }
        public string Type { get; set; }
    }
}
