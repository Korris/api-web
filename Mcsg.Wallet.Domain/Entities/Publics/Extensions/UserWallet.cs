namespace Mcsg.Wallet.Domain.Entities;

partial class UserWallet
{
    #region -- Properties --

    public WalletSetting WalletSetting { get; set; }
    public virtual ICollection<WalletTransaction> SourceUserWalletTransactions { get; set; }
    public virtual ICollection<WalletTransaction> DestinationUserWalletTransactions { get; set; }
    public virtual ICollection<PaymentMethod> PaymentMethods { get; set; }

    #endregion
}
