namespace Mcsg.Wallet.Domain.Entities;

using Common.SeedWork;

public partial class UserPurchaseTransaction : AuditableEntity
{
    public Guid WalletTransactionId { get; set; }
    public string? Title { get; set; }
    public string? Thumbnail { get; set; }
    public string? Paymethod { get; set; }
    public Guid? AffiliateUserId { get; set; }
    public Guid? CreatorUserId { get; set; }
    public bool IsPaid { get; set; }
}
