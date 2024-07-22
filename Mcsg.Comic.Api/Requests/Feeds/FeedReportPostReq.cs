namespace Mcsg.Comic.Api.Requests;

using Common.Core.Enums;

public class FeedReportPostReq
{
    public Guid PostId { get; set; }
    public ReasonType ReasonType { get; set; }
    public string? ReasonText { get; set; }
}
