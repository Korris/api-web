namespace Mcsg.Wallet.Api.Requests;

using Common.Core.Requests;
using Constants;

public class UserWalletWithdrawR : BaseR
{
    public float Amount { get; set; }
    public string Content { get; set; }
    public CurrencyType Type { get; set; }
    public Guid UserPaymentMethodId { get; set; }
}
