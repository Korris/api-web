namespace Mcsg.Story.Api.Models.Earning;

public class EarningDashboardResponse
{
    public EarningDashboardResponse()
    {
    }
    public float TotalGuestView { get; set; }
    public float TotalPremiumView { get; set; }
    public float SaleAffiliate { get; set; }
    public float SaleChapter { get; set; }
    public float RevenueOfMonth { get; set; }
    public float TotalRevenue { get; set; }
    public PerformanceChartResponse PerformanceChart { get; set; } = new PerformanceChartResponse();
}
