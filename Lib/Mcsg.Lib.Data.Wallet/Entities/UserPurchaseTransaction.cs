namespace Mcsg.Lib.Data.Wallet.Entities
{
    public class UserPurchaseTransaction : BaseWalletEntity
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
}
