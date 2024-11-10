using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

public partial class DocumentReport : BaseReport
{
    [InverseProperty("Report")]
    public virtual ICollection<DocumentReportDetail> DocumentReportDetails { get; set; } = new List<DocumentReportDetail>();
}
