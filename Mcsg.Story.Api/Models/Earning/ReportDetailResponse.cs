namespace Mcsg.Story.Api.Models.Earning;

public class ReportDetailResponse
{
    public string ComicName { get; set; }
    public List<ReportDetailData> Data { get; set; } = new List<ReportDetailData>();
}
