using Mcsg.Lib.Data.Wallet.Enums;

namespace Mcsg.Realtime.Api.DTOs
{
    public class TransactionUpdateNotiReq
    {
        public PaymentMethodType PaymentType { get; set; }
        public string TransactionId { get; set; } = string.Empty;
        public TransactionStatus TransactionStatus { get; set; }
    }
}
