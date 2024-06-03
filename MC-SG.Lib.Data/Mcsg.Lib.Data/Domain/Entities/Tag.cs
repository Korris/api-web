using Mcsg.Lib.Data.Domain.Entities.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Lib.Data.Domain.Entities
{
    [Table("Tags")]
    public class Tag : AuditableEntity
    {
        public Guid? AuthorId { get; set; }
        public string Title { get; set; }
        public string Name { get; set; }
    }
}