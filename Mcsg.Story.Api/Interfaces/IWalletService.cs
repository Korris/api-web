namespace Mcsg.Story.Api.Interfaces;

using Common.Core.Dtos;
using Models.Earning;

public interface IWalletService
{
    Task<int> GetTotalPurchaseOfChapterAsync(Guid chapterId, DateTime? date = null);
    Task<List<ChapterPurchaseDto>> GetPurchaseOfChaptersAsync(List<Guid> chapterIds, DateTime? date = null);
    Task<UserPurchaseData> GetRevenueSaleChapterOfUserAsync(Guid userId, DateTime? date = null);
    Task<List<RevenueChartData>> GetRevenueSaleChapterByYearAsync(Guid userId, int year);
    Task<List<RevenueChartData>> GetRevenueSaleChapterByMonthAsync(Guid userId, int month);
    Task<List<CountChartData>> GetNumberOfSaleChapterByYearAsync(Guid userId, int year);
    Task<List<CountChartData>> GetNumberOfSaleChapterByMonthAsync(Guid userId, int month);
}
