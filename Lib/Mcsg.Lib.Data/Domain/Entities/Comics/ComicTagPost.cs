using Mcsg.Lib.Data.Domain.Entities.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Lib.Data.Domain.Entities.Comics
{
    [Table("ComicTagPosts")]
    public class ComicTagPost : AuditableEntity
    {
        public Guid TagId { get; set; }
        public Guid PostId { get; set; }
    }
}