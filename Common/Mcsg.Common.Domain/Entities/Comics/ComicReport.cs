using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

public partial class ComicReport : BaseReport
{
    [InverseProperty("Report")]
    public virtual ICollection<ComicReportDetail> ComicReportDetails { get; set; } = new List<ComicReportDetail>();
}
