namespace Mcsg.Api.Areas.Document.Requests;

using Common.Core.Requests;

public class CommentReplyByCommentR : PaginatedR
{
    public Guid CommentId { get; set; }
    public bool IsSubPost { get; set; }
}
