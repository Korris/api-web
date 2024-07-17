namespace Mcsg.Comic.Api.Interfaces;

using Common.SeedWork.Responses;
using Models.Earning;
using Requests;

public interface IEarningService
{
    Task<bool> CheckUserEarningStatusAsync();
    Task EnableEarningAsync(EarningEnableR req);
    Task<EarningDashboardResponse> GetDataDashboardAsync();
    Task<List<MyPostSeriesResponse>> GetMyComicStoryListAsync();
    Task<PagedResponse<ReportSeriesData>> GetReportOfSeriesAsync(string seriesHashId, ComicChapterListR loadReq);
    string GetAffiliateCode(EarningAffiliateCodeR req);
    RevenueCalculateResponse CalculateRevenueOfMonth(RevenueCalculateReq data);
    Task<PerformanceChartResponse> GetPerformanceChart(Guid userId);
}
