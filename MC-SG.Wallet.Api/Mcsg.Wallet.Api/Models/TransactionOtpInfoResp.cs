using Mcsg.Lib.Data.Wallet.Enums;

namespace Mcsg.Wallet.Api.Models
{
    public class TransactionOtpInfoResp
    {
        public Guid TransactionId { get; set; }
        public string OtpToken { get; set; }
        public TransactionType Type { get; set; }
        public string Target { get; set; }
        public string ToProfileName { get; set; }
        public TransactionOtpType OtpType { get; set; }
    }
}
