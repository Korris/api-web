using Mcsg.Lib.Data.Domain.Entities.Common;
using Mcsg.Lib.Data.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Lib.Data.Domain.Entities
{
    [Table("Notifications")]
    public class Notification : AuditableEntity
    {
        public Guid NotificationObjectId { get; set; }
        public Guid? ReceiverId { get; set; }
        public NotificationStatus Status { get; set; }
    }
}
