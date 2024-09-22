using System.ComponentModel.DataAnnotations;

namespace Mcsg.Wallet.Domain.Entities;

using Enums;
using Mcsg.Common.SeedWork;

public class WalletSettingDetail : AuditableEntity
{
    [MaxLength(50)]
    public string? Name { get; set; }
    public WalletSettingDetailType Type { get; set; }
    public string? Value { get; set; }
    public string? Description { get; set; }
}
