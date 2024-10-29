namespace Mcsg.Document.Api.Models.Earning;

public class PerformanceChartEarningToDate
{
    public List<PerformanceChartEarningToDateData> YearData { get; set; } = new List<PerformanceChartEarningToDateData>();
    public List<PerformanceChartEarningToDateData> MonthData { get; set; } = new List<PerformanceChartEarningToDateData>();
}
