using Mcsg.Lib.Data.Domain.Entities.Common;
using Mcsg.Lib.Data.Enums;
using Mcsg.Lib.Model.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Lib.Data.Domain.Entities
{
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
