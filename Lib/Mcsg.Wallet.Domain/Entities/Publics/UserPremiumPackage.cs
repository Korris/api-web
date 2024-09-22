using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Wallet.Domain.Entities;

using Mcsg.Common.SeedWork;

public class UserPremiumPackage : AuditableEntity
{
    public int? PremiumPackageNo { get; set; }
    public virtual PremiumPackage PremiumPackage { get; set; }
    public Guid? UserWalletId { get; set; }
    public virtual UserWallet UserWallet { get; set; }

    [Column(TypeName = "timestamp")]
    public DateTime StartDate { get; set; }

    [Column(TypeName = "timestamp")]
    public DateTime EndDate { get; set; }
}
