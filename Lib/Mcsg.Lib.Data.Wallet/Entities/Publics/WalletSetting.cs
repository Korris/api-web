using System.ComponentModel.DataAnnotations;

namespace Mcsg.Lib.Data.Wallet.Entities;

using Mcsg.Common.SeedWork;

public class WalletSetting : AuditableEntity
{
    [MaxLength(50)]
    public string? Name { get; set; }
    [MaxLength(5)]
    public string? Symbol { get; set; }

    [MaxLength(255)]
    public string? Logo { get; set; }

    public virtual ICollection<UserWallet> Wallets { get; set; }
}
