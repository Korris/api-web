namespace Mcsg.Lib.Data.Wallet.Entities;

using Enums;
using Mcsg.Common.SeedWork;

public class WalletTransactionOtp : AuditableEntity
{
    public Guid TransactionId { get; set; }
    public WalletTransaction WalletTransaction { get; set; }
    public string? Otp { get; set; }
    public string? OtpToken { get; set; }
    public TransactionType Type { get; set; }
    public TransactionOtpType OtpType { get; set; }
}
