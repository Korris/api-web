using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

using SeedWork;
using Core.Enums;

[Table("Jobs")]
public class Job : AuditableEntity
{
    public JobType JobType { get; set; }
    public JobCategory JobCategory { get; set; }
    public string? Data { get; set; }
    public JobStatus Status { get; set; }
    public string? Error { get; set; }
}