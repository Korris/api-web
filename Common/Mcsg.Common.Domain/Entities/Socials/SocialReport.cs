using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

public partial class SocialReport : BaseReport
{
    [InverseProperty("Report")]
    public virtual ICollection<SocialReportDetail> SocialReportDetails { get; set; } = new List<SocialReportDetail>();
}
