namespace Mcsg.Wallet.Domain.Entities;

using Mcsg.Common.SeedWork;

public class UserPaymentMethod : AuditableEntity
{
    public Guid UserWalletId { get; set; }
    public virtual UserWallet UserWallet { get; set; }
    public Guid PaymentMethodId { get; set; }
    public virtual PaymentMethod PaymentMethod { get; set; }
    public string? AccountNumber { get; set; } // for banking, credit card or e-wallet
    public string? AccountName { get; set; } // for banking, credit card 
}
