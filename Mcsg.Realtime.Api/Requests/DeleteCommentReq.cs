
namespace Mcsg.Realtime.Api.Requests;

using Common.Core.Requests;

public class DeleteCommentReq : BaseR
{
    public Guid CommentId { get; set; }
    public string Type { get; set; }
}
