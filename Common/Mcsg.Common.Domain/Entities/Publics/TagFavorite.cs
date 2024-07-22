namespace Mcsg.Common.Domain.Entities;

using SeedWork;

public class TagFavorite : AuditableEntity
{
    public Guid TagId { get; set; }
    public Guid UserId { get; set; }
}
