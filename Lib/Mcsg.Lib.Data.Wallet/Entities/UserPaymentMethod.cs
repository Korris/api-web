namespace Mcsg.Lib.Data.Wallet.Entities
{
    public class UserPaymentMethod : BaseWalletEntity
    {
        public Guid UserWalletId { get; set; }
        public virtual UserWallet UserWallet { get; set; }
        public Guid PaymentMethodId { get; set; }
        public virtual PaymentMethod PaymentMethod { get; set; }
        public string AccountNumber { get; set; } // for banking, credit card or e-wallet
        public string AccountName { get; set; } // for banking, credit card 
    }
}
