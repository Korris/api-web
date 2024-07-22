using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities
{
    using Common;

    [Table("StoryTagPosts")]
    public class StoryTagPost : AuditableEntity
    {
        public Guid TagId { get; set; }
        public Guid PostId { get; set; }
    }
}