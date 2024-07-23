using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

using Core.Enums;
using SeedWork;

[Table("ComicSubPostComments")]
public class ComicSubPostComment : AuditableEntity
{
    public Guid? ParentId { get; set; }
    public Guid PostId { get; set; }
    public Guid AuthorId { get; set; }
    public int Order { get; set; }
    public string? Body { get; set; }
    public CommentStatus Status { get; set; }
    public Guid? ResourceId { get; set; }
    public string? GifId { get; set; }
}