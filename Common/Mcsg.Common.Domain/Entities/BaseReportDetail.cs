namespace Mcsg.Common.Domain.Entities;

using Core.Enums;
using SeedWork;

public class BaseReportDetail : AuditableEntity
{
    public Guid ReportId { get; set; }
    public Guid UserId { get; set; }
    public ReportDetailStatus Status { get; set; }
    public ReasonType ReasonType { get; set; }
    public string? ReasonText { get; set; }
}