namespace Mcsg.Common.Domain.Entities;

using SeedWork;

public class UserReferral : AuditableEntity
{
    public Guid UserReferrerId { get; set; }
    public Guid UserRefereeId { get; set; }
}
