namespace Mcsg.Common.Domain.Entities;

using Core.Enums;
using SeedWork;

public class NotificationObject : AuditableEntity
{
    public NotificationEntityType EntityType { get; set; }
    public NotificationAction Action { get; set; }
    public Guid? EntityId { get; set; }
    public string? EntityHashId { get; set; }
    public Guid? LocationId { get; set; }
    public string? LocationHashId { get; set; }
    public Guid ActorId { get; set; }
}
