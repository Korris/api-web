namespace Mcsg.Document.Api.Models.Earning;

public class ReportDetailResponse
{
    public string DocumentName { get; set; }
    public List<ReportDetailData> Data { get; set; } = new List<ReportDetailData>();
}
