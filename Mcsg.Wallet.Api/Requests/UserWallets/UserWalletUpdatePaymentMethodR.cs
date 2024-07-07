namespace Mcsg.Wallet.Api.Requests;

public class UserWalletUpdatePaymentMethodR : UserPaymentMethodReq
{
    public Guid? PaymentMethodId { get; set; }
}
