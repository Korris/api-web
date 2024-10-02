using System.ComponentModel.DataAnnotations;

namespace Mcsg.Wallet.Domain.Entities;

using Common.SeedWork;
using Common.SeedWork.Constants;

public class SystemSetting : AuditableEntity
{
    [StringLength(Validator.Description.Max)]
    public string? Description { get; set; }

    public string? Key { get; set; }
    public string? Value { get; set; }
    public bool IsActive { get; set; }
    public int Order { get; set; }

    public SystemSetting()
    {
        IsActive = true;
    }
}