namespace Mcsg.Wallet.Domain.Entities;

using Common.SeedWork;

public partial class UserPaymentMethod : AuditableEntity
{
    public Guid UserWalletId { get; set; }
    public Guid PaymentMethodId { get; set; }
    public string? AccountNumber { get; set; } // for banking, credit card or e-wallet
    public string? AccountName { get; set; } // for banking, credit card
}
