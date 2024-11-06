
namespace Mcsg.Realtime.Api.Requests;

using Common.Core.Requests;

public class DeleteReplyCommentReq : BaseR
{
    public Guid ReplyCommentId { get; set; }
    public Guid ReplyToCommentId { get; set; }
    public string Type { get; set; }
}
