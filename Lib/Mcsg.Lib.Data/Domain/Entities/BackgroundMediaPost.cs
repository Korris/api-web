using Mcsg.Lib.Data.Domain.Entities.Common;
using Mcsg.Lib.Data.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Lib.Data.Domain.Entities
{
    [Table("BackgroundMediaPosts")]
    public class BackgroundMediaPost : AuditableEntity
    {
        public Guid BackgroundMediaId { get; set; }
        public Guid PostId { get; set; }
        public BackgroundMediaPostStatus Status { get; set; }
    }
}
