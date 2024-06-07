using Mcsg.Lib.Data.Domain.Entities.Common;
using Mcsg.Lib.Data.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Lib.Data.Domain.Entities
{
    [Table("NotificationObjects")]
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
}
