namespace Mcsg.Wallet.Domain.Entities;

partial class UserPremiumPackage
{
    #region -- Properties --

    public virtual PremiumPackage PremiumPackage { get; set; }

    public virtual UserWallet UserWallet { get; set; }

    #endregion
}
