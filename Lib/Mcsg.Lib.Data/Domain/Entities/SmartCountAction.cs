using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Lib.Data.Domain.Entities
{
    using Lib.Data.Domain.Entities.Common;
    using Lib.Data.Enums;
    using Mcsg.Common.Core.Enums;

    [Table("SmartCountActions")]
    public class SmartCountAction : BaseEntity
    {
        public EntityType EntityType { get; set; }
        public EntitySubType? SubType { get; set; }
        public DateOnly Date { get; set; }
        public Guid EntityId { get; set; }
        public ActionType ActionType { get; set; }
        public int Count { get; set; }
        public DateTime? LastModifiedDate { get; set; }
    }
}
