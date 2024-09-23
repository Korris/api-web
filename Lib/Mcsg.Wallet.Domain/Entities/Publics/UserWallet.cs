using System.ComponentModel.DataAnnotations;

namespace Mcsg.Wallet.Domain.Entities;

using Common.SeedWork;
using Common.SeedWork.Enums;
using Enums;

public partial class UserWallet : AuditableEntity
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
    public UserWalletStatus Status { get; set; }
    public string? SystemMessage { get; set; }

    /// <summary>
    /// 0 Guest, 1 Free, 2 Premium, 3 Administrator
    /// </summary>
    public UserType Type { get; set; }
}
