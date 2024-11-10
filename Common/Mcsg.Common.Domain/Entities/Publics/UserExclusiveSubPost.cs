namespace Mcsg.Common.Domain.Entities;

using SeedWork;

public partial class UserExclusiveSubPost : AuditableEntity
{
    public Guid UserId { get; set; }
    public Guid SubPostId { get; set; }
}