namespace Mcsg.Social.Api.Requests
{
    public class CommentReplyByCommentR : BasePageResultReq
    {
        public Guid CommentId { get; set; }
        public bool IsSubPost { get; set; }
    }
}
