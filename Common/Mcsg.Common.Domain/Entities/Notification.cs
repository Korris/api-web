using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities
{
    using Common;
    using Core.Enums;

    [Table("Notifications")]
    public class Notification : AuditableEntity
    {
        public Guid NotificationObjectId { get; set; }
        public Guid? ReceiverId { get; set; }
        public NotificationStatus Status { get; set; }
    }
}
