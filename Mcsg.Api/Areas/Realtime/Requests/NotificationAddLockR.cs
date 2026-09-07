namespace Mcsg.Api.Areas.Realtime.Requests;

using Common.Core.Requests;

public class NotificationAddLockR : BaseR
{
    public Guid EntityId { get; set; }
    public string? NotificationType { get; set; }
}
