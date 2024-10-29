namespace Mcsg.Document.Api.Requests;

using Common.Core.Requests;

public class CommentReplyByCommentR : PaginatedR
{
    public Guid CommentId { get; set; }
    public bool IsSubPost { get; set; }
}
