using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

using SeedWork;
using Core.Enums;

[Table("Notifications")]
public class Notification : AuditableEntity
{
    public Guid NotificationObjectId { get; set; }
    public Guid? ReceiverId { get; set; }
    public NotificationStatus Status { get; set; }
}
