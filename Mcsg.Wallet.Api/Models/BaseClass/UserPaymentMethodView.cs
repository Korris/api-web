namespace Mcsg.Wallet.Api.Models.BaseClass;

using Mcsg.Common.Core.Enums;

public class UserPaymentMethodView
{
    public Guid UserWalletId { get; set; }
    public Guid PaymentMethodId { get; set; }
    public PaymentMethodType PaymentType { get; set; }
    public string Logo { get; set; }
    public string BankCode { get; set; }
    public string BankName { get; set; }
    public string AccountNumber { get; set; }
    public string AccountName { get; set; }
}
