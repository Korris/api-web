namespace Mcsg.Lib.Data.Wallet.Entities
{
    using Enums;
    using Mcsg.Common.SeedWork;

    public class EarningSummaryDetail : AuditableEntity
    {
        public Guid EarningSummaryId { get; set; }
        public virtual EarningSummary EarningSummary { get; set; }
        public EarningType Type { get; set; }
        public float DataValue { get; set; } //Log
        public float EarningValue { get; set; }
    }
}
