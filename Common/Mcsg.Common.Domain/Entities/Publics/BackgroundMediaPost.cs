using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

using Core.Enums;
using SeedWork;

[Table("BackgroundMediaPosts")]
public class BackgroundMediaPost : AuditableEntity
{
    public Guid BackgroundMediaId { get; set; }
    public Guid PostId { get; set; }
    public BackgroundMediaPostStatus Status { get; set; }
}
