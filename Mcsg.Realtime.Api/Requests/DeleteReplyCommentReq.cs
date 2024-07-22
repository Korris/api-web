namespace Mcsg.Realtime.Api.Requests;

public class DeleteReplyCommentReq
{
    public Guid ReplyCommentId { get; set; }
    public Guid ReplyToCommentId { get; set; }
    public string Type { get; set; }
    public string MicroService { get; set; } = Common.Core.Enums.MicroService.Social.ToString();
}
