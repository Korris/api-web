namespace Mcsg.Common.Domain.Entities;

using SeedWork;

public partial class TagFavorite : AuditableEntity
{
    public Guid TagId { get; set; }
    public Guid UserId { get; set; }
}
