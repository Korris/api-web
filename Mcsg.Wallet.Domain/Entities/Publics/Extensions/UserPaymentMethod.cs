namespace Mcsg.Wallet.Domain.Entities;
partial class UserPaymentMethod
{
    #region -- Properties --

    public virtual UserWallet UserWallet { get; set; }

    public virtual PaymentMethod PaymentMethod { get; set; }

    #endregion
}
