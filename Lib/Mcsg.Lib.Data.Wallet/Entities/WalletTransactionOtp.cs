using Mcsg.Lib.Data.Wallet.Enums;

namespace Mcsg.Lib.Data.Wallet.Entities
{
    public class WalletTransactionOtp : BaseWalletEntity
    {
        public Guid TransactionId { get; set; }
        public WalletTransaction WalletTransaction { get; set; }
        public string? Otp { get; set; }
        public string? OtpToken { get; set; }
        public TransactionType Type { get; set; }
        public TransactionOtpType OtpType { get; set; }
    }
}
