using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

public partial class SocialReportDetail : BaseReportDetail
{
    [ForeignKey("ReportId")]
    [InverseProperty("SocialReportDetails")]
    public virtual SocialReport Report { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("SocialReportDetails")]
    public virtual User User { get; set; } = null!;
}
