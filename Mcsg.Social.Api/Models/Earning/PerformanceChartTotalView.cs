namespace Mcsg.Social.Api.Models.Earning;

public class PerformanceChartTotalView
{
    public List<PerformanceChartTotalViewData> YearData { get; set; } = new List<PerformanceChartTotalViewData>();
    public List<PerformanceChartTotalViewData> MonthData { get; set; } = new List<PerformanceChartTotalViewData>();
}
