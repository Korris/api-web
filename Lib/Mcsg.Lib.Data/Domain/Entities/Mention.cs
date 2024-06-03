using Mcsg.Lib.Data.Domain.Entities.Common;
using Mcsg.Lib.Data.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Lib.Data.Domain.Entities
{
    [Table("Mentions")]
    public class Mention : AuditableEntity
    {
        public Guid LocationId { get; set; }
        public MentionLocationType LocationType { get; set; }
        public Guid EntityId { get; set; }
        public MentionEntityType EntityType { get; set; }
        public int Length { get; set; }
        public int Offset { get; set; }
        public string Text { get; set; }
    }
}
