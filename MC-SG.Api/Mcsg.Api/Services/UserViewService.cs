using Mcsg.Api.Extensions;
using Mcsg.Api.Models.Earning;
using Mcsg.Lib.Data.Analytic;
using Mcsg.Lib.Model.Enums;
using Mcsg.Lib.Model.Models;
using Microsoft.EntityFrameworkCore;

namespace Mcsg.Api.Services
{
    public interface IUserViewService
    {
        Task<EarningDataModel> GetGuestsViewAsync(Guid userId, DateTime? date = null);
        Task<EarningDataModel> GetPremiumViewAsync(Guid userId, DateTime? date = null);
        Task<int> GetViewByChapterAsync(Guid chapterId, DateTime? date = null);
        Task<List<ChapterViewData>> GetViewByChaptersAsync(List<Guid> chapterIds, DateTime? date = null);
        Task<List<PerformanceChartTotalViewData>> GetTotalViewChartByYear(Guid userId, int year);
        Task<List<PerformanceChartTotalViewData>> GetTotalViewChartByMonth(Guid userId, int month);
    }
    public class UserViewService : IUserViewService
    {
        private readonly AnalyticDbContext _dbAnalystContext;
        public UserViewService(AnalyticDbContext dbAnalystContext)
        {
            _dbAnalystContext = dbAnalystContext;
        }

        public async Task<EarningDataModel> GetGuestsViewAsync(Guid userId, DateTime? date = null)
        {
            var query = _dbAnalystContext.UserViewPosts.
                Where(x => x.AuthorId == userId
                && (x.UserType == Lib.Model.Enums.UserType.GUEST || x.UserType == Lib.Model.Enums.UserType.FREE));

            if (date != null && date.HasValue)
            {
                var firstDate = date.Value.FirstDate();
                var lastDate = date.Value.LastDate();

                query = query.Where(x => x.CreatedDate >= firstDate && x.CreatedDate <= lastDate);
            }

            var viewCount = await query.CountAsync();

            var data = new EarningDataModel()
            {
                Type = Enums.EarningDataType.TotalGuestView,
                Amount = viewCount,
            };

            return await Task.FromResult(data);
        }

        public async Task<EarningDataModel> GetPremiumViewAsync(Guid userId, DateTime? date = null)
        {
            var query = _dbAnalystContext.UserViewPosts.
                Where(x => x.AuthorId == userId && (x.UserType == Lib.Model.Enums.UserType.PREMIUM));

            if (date != null && date.HasValue)
            {
                var firstDate = date.Value.FirstDate();
                var lastDate = date.Value.LastDate();

                query = query.Where(x => x.CreatedDate >= firstDate && x.CreatedDate <= lastDate);
            }

            var viewCount = await query.CountAsync();

            var data = new EarningDataModel()
            {
                Type = Enums.EarningDataType.TotalPremiumView,
                Amount = viewCount,
            };


            return await Task.FromResult(data);
        }

        public async Task<int> GetViewByChapterAsync(Guid chapterId, DateTime? date = null)
        {
            var query = _dbAnalystContext.UserViewPosts.
                Where(x => x.SubPostId == chapterId);

            if (date != null && date.HasValue)
            {
                var firstDate = date.Value.FirstDate();
                var lastDate = date.Value.LastDate();

                query = query.Where(x => x.CreatedDate >= firstDate && x.CreatedDate <= lastDate);
            }

            var viewCount = await query.CountAsync();

            return viewCount;
        }

        public async Task<List<ChapterViewData>> GetViewByChaptersAsync(List<Guid> chapterIds, DateTime? date = null)
        {
            var query = _dbAnalystContext.UserViewPosts.
                Where(x => chapterIds.Contains(x.SubPostId));

            if (date != null && date.HasValue)
            {
                var firstDate = date.Value.FirstDate();
                var lastDate = date.Value.LastDate();

                query = query.Where(x => x.CreatedDate >= firstDate && x.CreatedDate <= lastDate);
            }

            var viewChapters = await query.GroupBy(x => x.SubPostId)
                                        .Select(n => new ChapterViewData
                                        {
                                            ChapterId = n.Key,
                                            Views = n.Count()
                                        }).ToListAsync();

            return viewChapters;
        }

        public async Task<List<PerformanceChartTotalViewData>> GetTotalViewChartByYear(Guid userId, int year)
        {
            var beginOfYear = year.BeginOfYear();
            var endOfYear = year.EndOfYear();
            var query = _dbAnalystContext.UserViewPosts
                 .Where(x => x.AuthorId == userId && x.CreatedDate >= beginOfYear && x.CreatedDate <= endOfYear)
                .GroupBy(v => new
                {
                    Year = v.CreatedDate.Year,
                    Month = v.CreatedDate.Month,
                    UserType = v.UserType == UserType.PREMIUM ? 0 : 1
                })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    UserType = g.Key.UserType,
                    ViewCount = g.Count()
                })
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .ThenBy(x => x.UserType);
            var yearView = await query.ToListAsync();

            var viewChart = new List<PerformanceChartTotalViewData>();

            for (int i = 1; i <= 12; i++)
            {
                var view = new PerformanceChartTotalViewData()
                {
                    Label = new DateTime(year, i, 1).ToLabel("MMMM")
                };
                viewChart.Add(view);
            }

            foreach (var item in yearView)
            {
                var label = new DateTime(item.Year, item.Month, 1).ToLabel("MMMM");
                var view = viewChart.Where(x => x.Label == label).FirstOrDefault();
                if (item.UserType == 0)
                {
                    view.Premium = item.ViewCount;
                }
                else
                {
                    view.FreeGuest = item.ViewCount;
                }
            }

            return viewChart;
        }
        public async Task<List<PerformanceChartTotalViewData>> GetTotalViewChartByMonth(Guid userId, int month)
        {
            var currentYear = DateTime.UtcNow.Year;
            var beginOfMonth = month.BeginOfMonth();
            var endOfMonth = month.EndOfMonth();
            var query = _dbAnalystContext.UserViewPosts
                 .Where(x => x.AuthorId == userId && x.CreatedDate >= beginOfMonth && x.CreatedDate <= endOfMonth)
                .GroupBy(v => new
                {
                    Year = v.CreatedDate.Year,
                    Month = v.CreatedDate.Month,
                    Day = v.CreatedDate.Day,
                    UserType = v.UserType == UserType.PREMIUM ? 0 : 1
                })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Day = g.Key.Day,
                    UserType = g.Key.UserType,
                    ViewCount = g.Count()
                })
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .ThenBy(x => x.Day)
                .ThenBy(x => x.UserType);
            var monthView = await query.ToListAsync();

            var viewChart = new List<PerformanceChartTotalViewData>();
            var maxDayInMonth = DateTime.DaysInMonth(currentYear, month);

            for (int i = 1; i <= maxDayInMonth; i++)
            {
                var view = new PerformanceChartTotalViewData()
                {
                    Label = new DateTime(currentYear, month, i).ToLabel("dd MMMM"),
                    FreeGuest = 0,
                    Premium = 0

                };
                viewChart.Add(view);
            }

            foreach (var item in monthView)
            {
                var label = new DateTime(item.Year, item.Month, item.Day).ToLabel("dd MMMM");
                var view = viewChart.Where(x => x.Label == label).FirstOrDefault();
                if (view != null)
                {
                    if (item.UserType == 0)
                    {
                        view.Premium = item?.ViewCount ?? 0;
                    }
                    else
                    {
                        view.FreeGuest = item?.ViewCount ?? 0;
                    }
                }
            }

            return viewChart;
        }
    }
}
