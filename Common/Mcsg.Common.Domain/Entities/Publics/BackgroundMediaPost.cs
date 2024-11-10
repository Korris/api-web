using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

using Core.Enums;
using SeedWork;

public partial class BackgroundMediaPost : AuditableEntity
{
    public Guid BackgroundMediaId { get; set; }

    public Guid PostId { get; set; }

    public BackgroundMediaPostStatus Status { get; set; }

    [ForeignKey("BackgroundMediaId")]
    [InverseProperty("BackgroundMediaPosts")]
    public virtual BackgroundMedia BackgroundMedia { get; set; } = null!;

    [ForeignKey("PostId")]
    [InverseProperty("BackgroundMediaPosts")]
    public virtual SocialPost Post { get; set; } = null!;
}
