namespace Mcsg.Common.Domain.Entities;

using Core.Enums;
using SeedWork;

public class Job : AuditableEntity
{
    public JobType JobType { get; set; }
    public JobCategory JobCategory { get; set; }
    public string? Data { get; set; }
    public JobStatus Status { get; set; }
    public string? Error { get; set; }
}