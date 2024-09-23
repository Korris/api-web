namespace Mcsg.Wallet.Domain.Entities;

partial class EarningPeriod
{
    #region -- Properties --

    public virtual ICollection<EarningSummary> EarningSummaries { get; set; }

    #endregion
}
