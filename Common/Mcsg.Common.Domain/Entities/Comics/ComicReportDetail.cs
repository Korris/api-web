using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

public partial class ComicReportDetail : BaseReportDetail
{
    [ForeignKey("ReportId")]
    [InverseProperty("ComicReportDetails")]
    public virtual ComicReport Report { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("ComicReportDetails")]
    public virtual User User { get; set; } = null!;
}
