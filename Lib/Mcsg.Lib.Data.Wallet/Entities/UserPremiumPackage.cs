namespace Mcsg.Lib.Data.Wallet.Entities
{
    public class UserPremiumPackage : BaseWalletEntity
    {
        public int? PremiumPackageNo { get; set; }
        public virtual PremiumPackage PremiumPackage { get; set; }
        public Guid? UserWalletId { get; set; }
        public virtual UserWallet UserWallet { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
