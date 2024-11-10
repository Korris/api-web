using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

public partial class StoryReportDetail : BaseReportDetail
{
    [ForeignKey("ReportId")]
    [InverseProperty("StoryReportDetails")]
    public virtual StoryReport Report { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("StoryReportDetails")]
    public virtual User User { get; set; } = null!;
}
