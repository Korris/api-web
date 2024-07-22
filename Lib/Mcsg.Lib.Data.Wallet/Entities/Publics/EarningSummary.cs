namespace Mcsg.Lib.Data.Wallet.Entities
{
    using Mcsg.Common.SeedWork;

    public class EarningSummary : AuditableEntity
    {
        public Guid UserId { get; set; }
        public Guid PeriodId { get; set; }
        public bool IsCompleted { get; set; }
        public float TotalAmount { get; set; } // Total amount 
        public virtual ICollection<EarningSummaryDetail> SummaryDetails { get; set; }
    }
}
