using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

using SeedWork;

[Table("SystemSettingHistories")]
public class SystemSettingHistory : AuditableEntity
{
    public Guid SystemSettingId { get; set; }
    public Guid UserId { get; set; }
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
}