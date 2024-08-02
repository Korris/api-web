namespace Mcsg.Common.Domain.Entities;

using SeedWork;

public class BaseFollowedPost : AuditableEntity
{
    public Guid PostId { get; set; }
}