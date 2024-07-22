using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

using Common;

[Table("Tags")]
public class Tag : AuditableEntity
{
    public Guid? AuthorId { get; set; }
    public string? Title { get; set; }
    public string? Name { get; set; }
}