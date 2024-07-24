namespace Mcsg.Common.Domain.Entities;

using SeedWork;

public class Tag : AuditableEntity
{
    public Guid? AuthorId { get; set; }
    public string? Title { get; set; }
    public string? Name { get; set; }
}