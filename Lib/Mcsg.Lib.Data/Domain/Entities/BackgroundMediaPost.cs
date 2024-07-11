using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Lib.Data.Domain.Entities
{
    using Common;
    using Mcsg.Common.Core.Enums;

    [Table("BackgroundMediaPosts")]
    public class BackgroundMediaPost : AuditableEntity
    {
        public Guid BackgroundMediaId { get; set; }
        public Guid PostId { get; set; }
        public BackgroundMediaPostStatus Status { get; set; }
    }
}
