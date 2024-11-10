using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

using SeedWork;

public partial class UserExclusiveSubPost : AuditableEntity
{
    public Guid UserId { get; set; }

    public Guid SubPostId { get; set; }

    [ForeignKey("SubPostId")]
    [InverseProperty("UserExclusiveSubPosts")]
    public virtual SocialSubPost SubPost { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("UserExclusiveSubPosts")]
    public virtual User User { get; set; } = null!;
}