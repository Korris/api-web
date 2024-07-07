namespace Mcsg.Wallet.Api.Requests;

public class UserWalletAddPaymentMethodR : UserPaymentMethodReq
{
    public Guid PaymentMethodId { get; set; }
}
