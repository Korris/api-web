namespace Mcsg.Realtime.Api.Requests;

using Common.Core.Enums;

public class FollowPostNotificationReq
{
    public string PostHashId { get; set; }
    public Guid PostId { get; set; }
    public Guid ActorId { get; set; }
    public Guid ReceiverId { get; set; }
    public string ActorName { get; set; }
    public string PostName { get; set; }
    public string UserAvatar { get; set; }
    public NotificationEntityType NotificationEntityType { get; set; }
}
