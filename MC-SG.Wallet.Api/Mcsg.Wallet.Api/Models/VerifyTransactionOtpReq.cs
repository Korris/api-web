namespace Mcsg.Wallet.Api.Models
{
    public class VerifyTransactionOtpReq
    {
        public Guid TransactionId { get; set; }
        public string Otp { get; set; }
        public string OtpToken { get; set; }
    }
}
