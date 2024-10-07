namespace Mcsg.Wallet.Domain.Entities;

using Common.Core.Enums;
using Common.SeedWork;

public class Job : AuditableEntity
{
    public JobType JobType { get; set; }
    public JobCategory JobCategory { get; set; }
    public string? Data { get; set; }
    public JobStatus Status { get; set; }
    public string? Error { get; set; }
}