using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

using SeedWork;
using SeedWork.Constants;

public partial class SystemSetting : AuditableEntity
{
    [StringLength(Validator.Description.Max)]
    public string? Description { get; set; }

    [StringLength(Validator.Name.Max)]
    public string? Key { get; set; }

    public string? Value { get; set; }
    public bool IsActive { get; set; }
    public int Order { get; set; }

    [StringLength(32)]
    public string? DataType { get; set; }

    [StringLength(32)]
    public string? Group { get; set; }

    [StringLength(32)]
    public string? MicroService { get; set; }

    [InverseProperty("SystemSetting")]
    public virtual ICollection<SystemSettingHistory> SystemSettingHistories { get; set; } = new List<SystemSettingHistory>();

    public SystemSetting()
    {
        IsActive = true;
    }
}