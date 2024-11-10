using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

using Core.Enums;
using SeedWork;

public partial class Notification : AuditableEntity
{
    public Guid NotificationObjectId { get; set; }

    public Guid? ReceiverId { get; set; }

    public NotificationStatus Status { get; set; }

    [ForeignKey("NotificationObjectId")]
    [InverseProperty("Notifications")]
    public virtual NotificationObject NotificationObject { get; set; } = null!;

    [ForeignKey("ReceiverId")]
    [InverseProperty("Notifications")]
    public virtual User? Receiver { get; set; }
}
