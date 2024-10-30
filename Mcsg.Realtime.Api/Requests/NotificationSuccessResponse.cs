namespace Mcsg.Realtime.Api.Requests;

public class NotificationSuccessResponse
{
    public Guid CommentId { get; set; }
    public string MicroService { get; set; }
    public Guid PostId { get; set; }
    public Guid? SubPostId { get; set; }
}
