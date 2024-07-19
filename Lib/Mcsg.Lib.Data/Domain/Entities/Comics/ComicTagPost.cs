using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Lib.Data.Domain.Entities
{
    using Common;

    [Table("ComicTagPosts")]
    public class ComicTagPost : AuditableEntity
    {
        public Guid TagId { get; set; }
        public Guid PostId { get; set; }
    }
}