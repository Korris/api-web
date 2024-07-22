namespace Mcsg.Common.Domain.Entities
{
    using Common;

    public class UserExclusiveSubPost : AuditableEntity
    {
        public Guid UserId { get; set; }
        public Guid SubPostId { get; set; }
    }
}