namespace Mcsg.Common.Domain.Entities;

using SeedWork;

public partial class UserFollow : AuditableEntity
{
    public Guid UserFollowerId { get; set; }
    public Guid UserFollowingId { get; set; }
}