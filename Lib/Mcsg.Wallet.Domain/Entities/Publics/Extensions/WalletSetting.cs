namespace Mcsg.Wallet.Domain.Entities;

partial class WalletSetting
{
    #region -- Properties --

    public virtual ICollection<UserWallet> Wallets { get; set; }

    #endregion
}
