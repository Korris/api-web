using Mcsg.Lib.Data.Domain.Entities.Common;

namespace Mcsg.Lib.Data.Domain.Entities
{
    public class UserFollow : AuditableEntity
    {
        public Guid UserFollowerId { get; set; }
        public Guid UserFollowingId { get; set; }
    }
}