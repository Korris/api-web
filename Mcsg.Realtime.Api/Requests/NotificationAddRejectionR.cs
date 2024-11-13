namespace Mcsg.Realtime.Api.Requests;

using Common.Core.Requests;

public class NotificationAddRejectionR : BaseR
{
    public Guid EntityId { get; set; }
    public string? EntityType { get; set; }
}
