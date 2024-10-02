namespace Mcsg.Wallet.Domain.Entities;

partial class PaymentMethod
{
    #region -- Properties --

    public virtual ICollection<UserPaymentMethod> UserPaymentMethods { get; set; }

    #endregion
}
