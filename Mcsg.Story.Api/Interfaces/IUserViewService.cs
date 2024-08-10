namespace Mcsg.Story.Api.Interfaces;

using Common.Core.Dtos;
using Models.Earning;

public interface IUserViewService
{
    Task<EarningDataModel> GetGuestsViewAsync(Guid userId, DateTime? date = null);
    Task<EarningDataModel> GetPremiumViewAsync(Guid userId, DateTime? date = null);
    Task<int> GetViewByChapterAsync(Guid chapterId, DateTime? date = null);
    Task<List<ChapterViewDto>> GetViewByChaptersAsync(List<Guid> chapterIds, DateTime? date = null);
    Task<List<PerformanceChartTotalViewData>> GetTotalViewChartByYear(Guid userId, int year);
    Task<List<PerformanceChartTotalViewData>> GetTotalViewChartByMonth(Guid userId, int month);
}
