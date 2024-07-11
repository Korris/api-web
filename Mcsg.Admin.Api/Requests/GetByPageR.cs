using System.ComponentModel;

namespace Mcsg.Admin.Api.Requests;

using Common.Core.Requests;

public class GetByPageR : PaginatedR
{
    [DefaultValue(true)]
    public bool OrderByAsc { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}
