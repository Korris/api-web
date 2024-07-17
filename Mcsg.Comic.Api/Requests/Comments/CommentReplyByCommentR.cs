namespace Mcsg.Comic.Api.Requests;

public class CommentReplyByCommentR : BasePageResultR
{
    public Guid CommentId { get; set; }
    public bool IsSubPost { get; set; }
}
