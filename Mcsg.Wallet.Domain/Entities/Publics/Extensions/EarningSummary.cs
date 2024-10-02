namespace Mcsg.Wallet.Domain.Entities;

partial class EarningSummary
{
    #region -- Properties --

    public virtual ICollection<EarningSummaryDetail> SummaryDetails { get; set; }

    #endregion
}
