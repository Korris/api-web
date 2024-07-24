namespace Mcsg.Common.Domain.Entities;

using SeedWork;

public class ComicTagPost : AuditableEntity
{
    public Guid TagId { get; set; }
    public Guid PostId { get; set; }
}