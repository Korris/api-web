namespace Mcsg.Api.Areas.Realtime.Requests;

using Common.Core.Requests;

public class NotificationAddRejectionR : BaseR
{
    public Guid EntityId { get; set; }
    public string? EntityType { get; set; }
}
