using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Wallet.Domain.Entities;

using Common.SeedWork;

public partial class UserPremiumPackage : AuditableEntity
{
    public int? PremiumPackageNo { get; set; }
    public Guid? UserWalletId { get; set; }

    [Column(TypeName = "timestamp")]
    public DateTime StartDate { get; set; }

    [Column(TypeName = "timestamp")]
    public DateTime EndDate { get; set; }
}
