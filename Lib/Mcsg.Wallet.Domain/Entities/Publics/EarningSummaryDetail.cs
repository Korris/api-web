namespace Mcsg.Wallet.Domain.Entities;

using Common.SeedWork;
using Enums;

public partial class EarningSummaryDetail : AuditableEntity
{
    public Guid EarningSummaryId { get; set; }
    public EarningType Type { get; set; }
    public float DataValue { get; set; } //Log
    public float EarningValue { get; set; }
}
