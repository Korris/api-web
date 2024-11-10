using System.ComponentModel.DataAnnotations.Schema;
namespace Mcsg.Common.Domain.Entities;

using SeedWork;

public partial class SystemSettingHistory : AuditableEntity
{
    public Guid SystemSettingId { get; set; }

    public Guid UserId { get; set; }

    public string? OldValue { get; set; }

    public string? NewValue { get; set; }

    [ForeignKey("SystemSettingId")]
    [InverseProperty("SystemSettingHistories")]
    public virtual SystemSetting SystemSetting { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("SystemSettingHistories")]
    public virtual User User { get; set; } = null!;
}