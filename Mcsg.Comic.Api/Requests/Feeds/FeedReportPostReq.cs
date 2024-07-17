namespace Mcsg.Comic.Api.Requests;

using Lib.Data.Enums;

public class FeedReportPostReq
{
    public Guid PostId { get; set; }
    public ReasonType ReasonType { get; set; }
    public string? ReasonText { get; set; }
}
