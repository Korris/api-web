namespace Mcsg.Wallet.Domain.Entities;

partial class WalletTransaction
{
    #region -- Properties --

    public virtual UserWallet SourceUserWallet { get; set; }
    public virtual UserWallet DestinationUserWallet { get; set; }
    public virtual UserPaymentMethod UserPaymentMethods { get; set; }
    public virtual ICollection<WalletTransactionOtp> WalletTransactionOtps { get; set; }
    public virtual ICollection<UserPurchaseTransaction> UserPurchaseTransactions { get; set; }

    #endregion
}
