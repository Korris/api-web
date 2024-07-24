namespace Mcsg.Common.Domain.Entities;

using Core.Enums;
using SeedWork;

public class ComicPostReport : AuditableEntity
{
    public Guid PostId { get; set; }
    public Guid UserId { get; set; }
    public ReasonType ReasonType { get; set; }
    public string? ReasonText { get; set; }
}
