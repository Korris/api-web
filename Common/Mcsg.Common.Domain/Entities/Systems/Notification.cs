namespace Mcsg.Common.Domain.Entities;

using Core.Enums;
using SeedWork;

public class Notification : AuditableEntity
{
    public Guid NotificationObjectId { get; set; }
    public Guid? ReceiverId { get; set; }
    public NotificationStatus Status { get; set; }
}
