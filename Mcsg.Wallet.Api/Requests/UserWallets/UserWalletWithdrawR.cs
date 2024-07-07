namespace Mcsg.Wallet.Api.Requests;

using Constants;

public class UserWalletWithdrawR
{
    public float Amount { get; set; }
    public string Content { get; set; }
    public CurrencyType Type { get; set; }
    public Guid UserPaymentMethodId { get; set; }
}
