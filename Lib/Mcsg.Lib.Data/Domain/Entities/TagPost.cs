using Mcsg.Lib.Data.Domain.Entities.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Lib.Data.Domain.Entities
{
    [Table("TagPosts")]
    public class TagPost : AuditableEntity
    {
        public Guid TagId { get; set; }
        public Guid PostId { get; set; }
    }
}