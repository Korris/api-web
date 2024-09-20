using System.ComponentModel.DataAnnotations;

namespace Mcsg.Lib.Data.Wallet.Entities;

using Enums;
using Mcsg.Common.SeedWork;

public class UserWallet : AuditableEntity
{
    public Guid UserId { get; set; }

    [MaxLength(255)]
    public string? ProfileName { get; set; }

    [MaxLength(255)]
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }

    [MaxLength(20)]
    public string? Address { get; set; }
    public float Point { get; set; }
    public float RewardPoint { get; set; }
    public Guid WalletSettingId { get; set; }
    public WalletSetting WalletSetting { get; set; }
    public virtual ICollection<WalletTransaction> SourceUserWalletTransactions { get; set; }
    public virtual ICollection<WalletTransaction> DestinationUserWalletTransactions { get; set; }

    public virtual ICollection<PaymentMethod> PaymentMethods { get; set; }
    public UserWalletStatus Status { get; set; }
    public string? SystemMessage { get; set; }
}
