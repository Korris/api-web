namespace Mcsg.Realtime.Api.Requests;

public class UpdateCommentReq : PostCommentReq
{
    public Guid CommentId { get; set; }
    public string MicroService { get; set; } = Common.Core.Enums.MicroService.Social.ToString();
}
