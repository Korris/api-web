namespace Mcsg.Realtime.Api.Requests;

using Lib.Data.Wallet.Enums;

public class TransactionUpdateNotiReq
{
    public PaymentMethodType PaymentType { get; set; }
    public string TransactionId { get; set; } = string.Empty;
    public TransactionStatus TransactionStatus { get; set; }
}
