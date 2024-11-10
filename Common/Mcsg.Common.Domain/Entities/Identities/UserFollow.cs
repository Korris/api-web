using System.ComponentModel.DataAnnotations.Schema;
namespace Mcsg.Common.Domain.Entities;

using SeedWork;

public partial class UserFollow : AuditableEntity
{
    public Guid UserFollowerId { get; set; }
    public Guid UserFollowingId { get; set; }

    [ForeignKey("UserFollowerId")]
    [InverseProperty("UserFollowUserFollowers")]
    public virtual User UserFollower { get; set; } = null!;

    [ForeignKey("UserFollowingId")]
    [InverseProperty("UserFollowUserFollowings")]
    public virtual User UserFollowing { get; set; } = null!;
}
