namespace Mcsg.Social.Api.Models.Earning;

public class PerformanceChartTotalPurchase
{
    public List<PerformanceChartTotalPurchaseData> YearData { get; set; } = new List<PerformanceChartTotalPurchaseData>();
    public List<PerformanceChartTotalPurchaseData> MonthData { get; set; } = new List<PerformanceChartTotalPurchaseData>();
}
