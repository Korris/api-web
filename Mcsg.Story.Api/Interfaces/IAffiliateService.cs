namespace Mcsg.Story.Api.Interfaces;

using Models.Earning;

public interface IAffiliateService
{
    Task<EarningDataModel> GetSaleAffiliateAsync(Guid userId, DateTime? date = null);
    string GetAffiliateCode(Guid userId, EarningAffiliateCodeR req);
    Task<List<RevenueChartData>> GetRevenueAffiliateByYearAsync(Guid userId, int year);
    Task<List<RevenueChartData>> GetRevenueAffiliateByMonthAsync(Guid userId, int month);
    Task<List<CountChartData>> GetNumberOfAffiliateByYearAsync(Guid userId, int year);
    Task<List<CountChartData>> GetNumberOfAffiliateByMonthAsync(Guid userId, int month);
}
