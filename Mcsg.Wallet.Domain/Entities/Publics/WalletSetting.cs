using System.ComponentModel.DataAnnotations;

namespace Mcsg.Wallet.Domain.Entities;

using Common.SeedWork;

public partial class WalletSetting : AuditableEntity
{
    [MaxLength(50)]
    public string? Name { get; set; }
    [MaxLength(5)]
    public string? Symbol { get; set; }

    [MaxLength(255)]
    public string? Logo { get; set; }
}
