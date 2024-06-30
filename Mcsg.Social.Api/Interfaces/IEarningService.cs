namespace Mcsg.Social.Api.Interfaces
{
    using Lib.Data.Entities.Common;
    using Models;
    using Models.Earning;
    using Requests;

    public interface IEarningService
    {
        Task<bool> CheckUserEarningStatusAsync();
        Task EnableEarningAsync(EnableEarningModeRequest req);
        Task<EarningDashboardResponse> GetDataDashboardAsync();
        Task<List<MyPostSeriesResponse>> GetMyComicStoryListAsync();
        Task<PagedResults<ReportSeriesData>> GetReportOfSeriesAsync(string seriesHashId, ChapterListReq loadReq);
        string GetAffiliateCode(AffiliateCodeRequest req);
        RevenueCalculateResponse CalculateRevenueOfMonth(RevenueCalculateReq data);
        Task<PerformanceChartResponse> GetPerformanceChart(Guid userId);
    }
}
