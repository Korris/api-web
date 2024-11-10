using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

public partial class DocumentReportDetail : BaseReportDetail
{
    [ForeignKey("ReportId")]
    [InverseProperty("DocumentReportDetails")]
    public virtual DocumentReport Report { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("DocumentReportDetails")]
    public virtual User User { get; set; } = null!;
}
