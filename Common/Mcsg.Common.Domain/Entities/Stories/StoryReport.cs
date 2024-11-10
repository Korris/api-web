using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

public partial class StoryReport : BaseReport
{
    [InverseProperty("Report")]
    public virtual ICollection<StoryReportDetail> StoryReportDetails { get; set; } = new List<StoryReportDetail>();
}
