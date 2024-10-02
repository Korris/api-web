namespace Mcsg.Wallet.Domain.Entities;

using Common.SeedWork;
using Enums;

public partial class WalletTransactionOtp : AuditableEntity
{
    public Guid TransactionId { get; set; }
    public string? Otp { get; set; }
    public string? OtpToken { get; set; }
    public TransactionType Type { get; set; }
    public TransactionOtpType OtpType { get; set; }
}
