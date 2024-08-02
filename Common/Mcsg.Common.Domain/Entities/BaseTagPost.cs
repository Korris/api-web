namespace Mcsg.Common.Domain.Entities;

using SeedWork;

public class BaseTagPost : AuditableEntity
{
    public Guid TagId { get; set; }
    public Guid PostId { get; set; }
}