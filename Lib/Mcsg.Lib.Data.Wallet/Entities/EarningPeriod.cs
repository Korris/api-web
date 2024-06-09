using System.ComponentModel.DataAnnotations;

namespace Mcsg.Lib.Data.Wallet.Entities
{
    public class EarningPeriod : BaseWalletEntity
    {
        [MaxLength(50)]
        public string? Title { get; set; } // 2024-001 => Year - number of payment
        public int Year { get; set; }
        public int Order { get; set; } // Manual
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public virtual ICollection<EarningSummary> EarningSummaries { get; set; }
    }
}
