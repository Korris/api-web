namespace Mcsg.Wallet.Api.Models;

using Lib.Data.Wallet.Enums;

public class UserPaymentMethodResp
{
    public Guid Id { get; set; }
    public PaymentMethodType PaymentMethodType { get; set; }
    public Guid PaymentMethodId { get; set; }
    public string PaymentMethodName { get; set; }
}
