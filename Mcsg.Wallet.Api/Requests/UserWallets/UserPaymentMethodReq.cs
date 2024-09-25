namespace Mcsg.Wallet.Api.Requests;

using Common.Core.Requests;

public class UserPaymentMethodReq : BaseR
{
    public string AccountNumber { get; set; }
    public string AccountName { get; set; }
}

