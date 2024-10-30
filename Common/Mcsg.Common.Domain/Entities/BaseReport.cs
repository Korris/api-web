namespace Mcsg.Common.Domain.Entities;

using Core.Enums;
using SeedWork;

public class BaseReport : AuditableEntity
{
    public Guid EntityId { get; set; }
    public EntityType EntityType { get; set; }
    public ReportStatus Status { get; set; }
    public DateTime? ExpiredBlock { get; set; }
}
