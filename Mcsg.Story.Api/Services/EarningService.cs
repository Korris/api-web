using AutoMapper;

namespace Mcsg.Story.Api.Services;

using Common.Domain.Entities;
using Common.SeedWork.Responses;
using Constants;
using Extensions;
using Interfaces;
using Lib.Common.Extensions;
using Lib.Common.Web.Security;
using Lib.Data.Repositories;
using Models.Earning;
using Requests;

public class EarningService : IEarningService
{
    private readonly IRepository<User> _userRepository;
    private readonly ICurrentUserService _currentUserService;
    private IConfiguration _configuration;
    private readonly ILogger<EarningService> _logger;
    private readonly IAffiliateService _affiliateService;
    private readonly IPostService _postService;
    private readonly IWalletService _walletService;
    private readonly IMapper _mapper;
    public EarningService(IRepository<User> userRepository,
        ICurrentUserService currentUserService,
        IAffiliateService affiliateService,
        IPostService postService,
        IWalletService walletService,
        IConfiguration configuration,
        IMapper mapper,
        ILogger<EarningService> logger)
    {
        _userRepository = userRepository;
        _currentUserService = currentUserService;
        _affiliateService = affiliateService;
        _postService = postService;
        _walletService = walletService;
        _configuration = configuration;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<bool> CheckUserEarningStatusAsync()
    {
        var user = await _userRepository.GetByIdAsync(_currentUserService.Session.UserId);
        return user != null ? user.IsActiveEarning : false;
    }

    public async Task EnableEarningAsync(EarningEnableR req)
    {
        var user = await _userRepository.GetByIdAsync(_currentUserService.Session.UserId);
        if (user != null)
        {
            user.IsActiveEarning = req.Status;
            await _userRepository.UpdateAsync(user);
        }
    }
    public async Task<EarningDashboardResponse> GetDataDashboardAsync()
    {
        var userId = _currentUserService.Session.UserId;

        //Get Total Guest View
        var totalGuestView = new EarningDataModel { Amount = 0 }; //TODO Analytic await _userViewService.GetGuestsViewAsync(userId, DateTime.Now);

        //Get Total Premium View
        var totalPremiumView = new EarningDataModel { Amount = 0 }; //TODO Analytic await _userViewService.GetPremiumViewAsync(userId, DateTime.Now);

        //Get Sale Affiliate
        var saleAffiliate = await _affiliateService.GetSaleAffiliateAsync(userId, DateTime.Now);

        //Get Sale Chapter
        var saleChapter = await _walletService.GetRevenueSaleChapterOfUserAsync(userId, DateTime.Now);

        //Get Revenue Of Month
        var revenueReq = new RevenueCalculateReq()
        {
            GuestView = totalGuestView != null ? totalGuestView.Amount : 0,
            PremiumView = totalPremiumView != null ? totalPremiumView.Amount : 0,
            SaleAffiliate = saleAffiliate != null ? saleAffiliate.Amount : 0,
            SaleChapter = saleChapter != null ? saleChapter.Amount : 0
        };
        var revenue = CalculateRevenueOfMonth(revenueReq);

        var dashBoardData = new EarningDashboardResponse()
        {
            TotalGuestView = totalGuestView != null ? totalGuestView.Amount : 0, // Display view, not point earn from view
            TotalPremiumView = totalPremiumView != null ? totalPremiumView.Amount : 0, // Display view, not point earn from view
            SaleAffiliate = revenue.SaleAffiliatePoint,
            SaleChapter = revenue.SaleChapterPoint,
            RevenueOfMonth = revenue.TotalRevenuePoint
        };

        //Get Total Revenue
        // TO DO LATER
        dashBoardData.TotalRevenue = dashBoardData.RevenueOfMonth;

        //Get Performance chart
        dashBoardData.PerformanceChart = await GetPerformanceChart(userId);

        return dashBoardData;
    }
    public async Task<List<MyPostSeriesResponse>> GetMyComicStoryListAsync()
    {
        return await _postService.GetMyAllSeries();
    }
    public async Task<PagedResponse<ReportSeriesData>> GetReportOfSeriesAsync(string seriesHashId, ComicChapterListR loadReq)
    {
        var userId = _currentUserService.Session.UserId;

        var chapterPageData = await _postService.GetChaptersListSimple(userId, seriesHashId, loadReq);
        if (chapterPageData != null && chapterPageData.TotalItems > 0)
        {
            var chapters = chapterPageData.Items;
            var dtos = _mapper.Map<List<ReportSeriesData>>(chapters);

            var chapterIds = dtos.Select(i => i.Id).ToList();
            /* Get View of chapters //TODO Analytic
            var chapterViews = await _userViewService.GetViewByChaptersAsync(chapterIds);
            if (chapterViews != null)
            {
                foreach (var chapterView in chapterViews)
                {
                    var chapterData = dtos.FirstOrDefault(x => x.Id == chapterView.ChapterId);
                    chapterData.Views = chapterView.Views;
                }
            }*/

            // Get Purchase of chapters
            var chapterPurchases = await _walletService.GetPurchaseOfChaptersAsync(chapterIds);
            if (chapterPurchases != null)
            {
                foreach (var chapter in chapterPurchases)
                {
                    var chapterData = dtos.FirstOrDefault(x => x.Id == chapter.ChapterId);
                    chapterData.Purchases = chapter.Purchases;
                }
            }

            var results = new PagedResponse<ReportSeriesData>(chapterPageData.TotalItems, loadReq.PageNumber, loadReq.PageSize);
            results.Items = dtos;
            return results;
        }
        else
        {
            return new PagedResponse<ReportSeriesData>(0);
        }
    }
    public string GetAffiliateCode(EarningAffiliateCodeR req)
    {
        return _affiliateService.GetAffiliateCode(_currentUserService.Session.UserId, req);
    }
    public RevenueCalculateResponse CalculateRevenueOfMonth(RevenueCalculateReq data)
    {
        var revenueFreeView = data.GuestView * EarningConst.FreeViewRate;
        var revenuePremiumView = data.PremiumView * EarningConst.PremiumViewRate;
        var revenueAffiliate = data.SaleAffiliate.Percentage(EarningConst.RevenueAffiliatePercent);
        var revenueSaleChapter = data.SaleChapter.Percentage(EarningConst.RevenueCreatorPercent);

        var total = revenueFreeView + revenuePremiumView + revenueAffiliate + revenueSaleChapter;

        return new RevenueCalculateResponse()
        {
            GuestViewPoint = revenueFreeView,
            PremiumViewPoint = revenuePremiumView,
            SaleAffiliatePoint = revenueAffiliate,
            SaleChapterPoint = revenueSaleChapter,
            TotalRevenuePoint = total
        };
    }

    public async Task<PerformanceChartResponse> GetPerformanceChart(Guid userId)
    {
        var report = new PerformanceChartResponse();
        var currentDate = DateTime.UtcNow;

        // Fake data
        Random rnd = new Random();
        var years = new List<string>();
        for (int i = 1; i < 13; i++)
        {
            var dt = new DateTime(currentDate.Year, i, 1);
            years.Add(dt.ToLabel("MMMM"));
        }

        var months = new List<string>();
        var lastDay = currentDate.LastDay();
        for (int i = 1; i < (lastDay + 1); i++)
        {
            var dt = new DateTime(currentDate.Year, currentDate.Month, i);
            months.Add(dt.ToLabel("dd MMMM"));
        }

        //TODO Analytic
        report.TotalView = new PerformanceChartTotalView()
        {
            YearData = [],// await _userViewService.GetTotalViewChartByYear(userId, currentDate.Year),
            MonthData = []// await _userViewService.GetTotalViewChartByMonth(userId, currentDate.Month)
        };

        report.TotalPurchase = new PerformanceChartTotalPurchase()
        {
            YearData = await GetPurchaseChartYearData(userId, currentDate.Year),
            MonthData = await GetPurchaseChartMonthData(userId, currentDate.Month),
        };

        report.EarningToDate = new PerformanceChartEarningToDate()
        {
            YearData = GetEarningToDateChartYearData(currentDate.Year, report.TotalView.YearData, report.TotalPurchase.YearData),
            MonthData = GetEarningToDateChartMonthData(currentDate.Month, report.TotalView.MonthData, report.TotalPurchase.MonthData),
        };

        return report;
    }

    private async Task<List<PerformanceChartTotalPurchaseData>> GetPurchaseChartYearData(Guid userId, int year)
    {
        var yearSaleData = await _walletService.GetRevenueSaleChapterByYearAsync(userId, year);
        var yearAffiliateData = await _affiliateService.GetRevenueAffiliateByYearAsync(userId, year);

        var chartData = new List<PerformanceChartTotalPurchaseData>();

        for (int i = 1; i <= 12; i++)
        {
            var data = new PerformanceChartTotalPurchaseData()
            {
                Label = new DateTime(year, i, 1).ToLabel("MMMM")
            };
            chartData.Add(data);
        }

        // Số lượt mua chapter
        foreach (var item in yearSaleData)
        {
            var label = new DateTime(item.Year, item.Month, 1).ToLabel("MMMM");
            var data = chartData.Where(x => x.Label == label).FirstOrDefault();

            data.SaleIndividual = item.Amount.Percentage(EarningConst.RevenueCreatorPercent);
        }

        // Số lượt mua qua Affiliate
        foreach (var item in yearAffiliateData)
        {
            var label = new DateTime(item.Year, item.Month, 1).ToLabel("MMMM");
            var data = chartData.Where(x => x.Label == label).FirstOrDefault();

            data.SalePremium = item.Amount.Percentage(EarningConst.RevenueAffiliatePercent);
        }

        return chartData;
    }

    private async Task<List<PerformanceChartTotalPurchaseData>> GetPurchaseChartMonthData(Guid userId, int month)
    {
        var currentYear = DateTime.UtcNow.Year;
        var yearSaleData = await _walletService.GetRevenueSaleChapterByMonthAsync(userId, month);
        var yearAffiliateData = await _affiliateService.GetRevenueAffiliateByMonthAsync(userId, month);

        var chartData = new List<PerformanceChartTotalPurchaseData>();

        var maxDayInMonth = DateTime.DaysInMonth(currentYear, month);

        for (int i = 1; i <= maxDayInMonth; i++)
        {
            var data = new PerformanceChartTotalPurchaseData()
            {
                Label = new DateTime(currentYear, month, i).ToLabel("dd MMMM"),
                SaleIndividual = 0,
                SalePremium = 0

            };
            chartData.Add(data);
        }

        // Số lượt mua chapter
        foreach (var item in yearSaleData)
        {
            var label = new DateTime(item.Year, item.Month, item.Day).ToLabel("dd MMMM");
            var data = chartData.Where(x => x.Label == label).FirstOrDefault();
            if (data != null)
            {
                data.SaleIndividual = item.Amount.Percentage(EarningConst.RevenueCreatorPercent);
            }
        }

        // Số lượt mua qua Affiliate
        foreach (var item in yearAffiliateData)
        {
            var label = new DateTime(item.Year, item.Month, item.Day).ToLabel("dd MMMM");
            var data = chartData.Where(x => x.Label == label).FirstOrDefault();
            if (data != null)
            {
                data.SalePremium = item.Amount.Percentage(EarningConst.RevenueAffiliatePercent);
            }
        }

        return chartData;
    }

    private List<PerformanceChartEarningToDateData> GetEarningToDateChartYearData(int year, List<PerformanceChartTotalViewData> totalView, List<PerformanceChartTotalPurchaseData> totalPurchase)
    {
        var chartData = new List<PerformanceChartEarningToDateData>();

        for (int i = 1; i <= 12; i++)
        {
            var label = new DateTime(year, i, 1).ToLabel("MMMM");
            var viewData = totalView.Where(x => x.Label == label).FirstOrDefault();
            var purchaseData = totalPurchase.Where(x => x.Label == label).FirstOrDefault();

            float viewAmount = (viewData.FreeGuest * EarningConst.FreeViewRate) + (viewData.Premium * EarningConst.PremiumViewRate);

            var data = new PerformanceChartEarningToDateData()
            {
                Label = new DateTime(year, i, 1).ToLabel("MMMM"),
                Quantity = viewAmount + purchaseData.SalePremium + purchaseData.SaleIndividual

            };
            chartData.Add(data);
        }

        return chartData;
    }

    private List<PerformanceChartEarningToDateData> GetEarningToDateChartMonthData(int month, List<PerformanceChartTotalViewData> totalView, List<PerformanceChartTotalPurchaseData> totalPurchase)
    {
        var currentYear = DateTime.UtcNow.Year;
        var chartData = new List<PerformanceChartEarningToDateData>();

        var maxDayInMonth = DateTime.DaysInMonth(currentYear, month);

        for (int i = 1; i <= maxDayInMonth; i++)
        {
            var label = new DateTime(currentYear, month, i).ToLabel("dd MMMM");
            var viewData = totalView.Where(x => x.Label == label).FirstOrDefault();
            var purchaseData = totalPurchase.Where(x => x.Label == label).FirstOrDefault();

            float viewAmount = (viewData.FreeGuest * EarningConst.FreeViewRate) + (viewData.Premium * EarningConst.PremiumViewRate);

            var data = new PerformanceChartEarningToDateData()
            {
                Label = label,
                Quantity = viewAmount + purchaseData.SalePremium + purchaseData.SaleIndividual
            };
            chartData.Add(data);
        }

        return chartData;
    }
}
