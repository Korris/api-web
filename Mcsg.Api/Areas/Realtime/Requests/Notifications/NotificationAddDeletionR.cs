namespace Mcsg.Api.Areas.Realtime.Requests;

using Common.Core.Requests;

public class NotificationAddDeletionR : BaseR
{
    public Guid EntityId { get; set; }
    public string? NotificationType { get; set; }
}
