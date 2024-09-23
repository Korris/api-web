namespace Mcsg.Wallet.Domain.Entities;

using Common.SeedWork;

public partial class EarningSummary : AuditableEntity
{
    public Guid UserId { get; set; }
    public Guid PeriodId { get; set; }
    public bool IsCompleted { get; set; }
    public float TotalAmount { get; set; } // Total amount
}
