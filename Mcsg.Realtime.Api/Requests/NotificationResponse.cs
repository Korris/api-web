namespace Mcsg.Realtime.Api.Requests;

public class NotificationResponse
{
    public Guid Id { get; set; }
    public string Status { get; set; }
    public string TargetType { get; set; }
    public Guid? LocationId { get; set; }
    public string LocationHashId { get; set; }
    public Guid? EntityId { get; set; }
    public string EntityHashId { get; set; }
    public string Message { get; set; }
    public Guid ActorId { get; set; }
    public string ActorName { get; set; }
    public DateTime CreatedOn { get; set; }
    public string NotificationType { get; set; }
    public string UserAvatar { get; set; }
}
