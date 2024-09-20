namespace Mcsg.Lib.Data.Wallet.Entities;

using Mcsg.Common.SeedWork;

public class UserPurchaseTransaction : AuditableEntity
{
    public Guid WalletTransactionId { get; set; }
    public WalletTransaction WalletTransaction { get; set; }
    public string? Title { get; set; }
    public string? Thumbnail { get; set; }
    public string? Paymethod { get; set; }
    public Guid? AffiliateUserId { get; set; }
    public Guid? CreatorUserId { get; set; }
    public bool IsPaid { get; set; }
}
