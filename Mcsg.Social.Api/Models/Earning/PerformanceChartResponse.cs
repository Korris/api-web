namespace Mcsg.Api.Models.Earning
{
    public class PerformanceChartResponse
    {
        public PerformanceChartTotalView TotalView { get; set; }
        public PerformanceChartTotalPurchase TotalPurchase { get; set; }
        public PerformanceChartEarningToDate EarningToDate { get; set; }
    }
}
