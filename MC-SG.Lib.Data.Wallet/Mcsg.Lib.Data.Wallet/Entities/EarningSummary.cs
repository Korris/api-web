namespace Mcsg.Lib.Data.Wallet.Entities
{
    public class EarningSummary : BaseWalletEntity
    {
        public Guid UserId { get; set; }
        public Guid PeriodId { get; set; }
        public bool IsCompleted { get; set; }
        public float TotalAmount { get; set; } // Total amount 
        public virtual ICollection<EarningSummaryDetail> SummaryDetails { get; set; }
    }
}
