namespace Mcsg.Common.Domain.Entities
{
    using Common;

    public class UserFollow : AuditableEntity
    {
        public Guid UserFollowerId { get; set; }
        public Guid UserFollowingId { get; set; }
    }
}