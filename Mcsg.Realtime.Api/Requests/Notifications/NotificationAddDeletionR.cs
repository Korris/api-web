namespace Mcsg.Realtime.Api.Requests;

using Common.Core.Requests;

public class NotificationAddDeletionR : BaseR
{
    public Guid EntityId { get; set; }
    public string? NotificationType { get; set; }
}
