using Mcsg.Lib.Data.Domain.Entities.Common;

namespace Mcsg.Lib.Data.Domain.Entities
{
    public class UserExclusiveSubPost : AuditableEntity
    {
        public Guid UserId { get; set; }
        public Guid SubPostId { get; set; }
    }
}