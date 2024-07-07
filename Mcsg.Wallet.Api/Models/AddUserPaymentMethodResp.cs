namespace Mcsg.Wallet.Api.Models;

using Requests;

public class AddUserPaymentMethodResp : UserWalletAddPaymentMethodR
{
    public Guid Id { get; set; }
}

public class UpdateUserPaymentMethodResp : UserWalletAddPaymentMethodR
{
    public Guid Id { get; set; }
}
