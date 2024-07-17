using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Mcsg.Story.Api.Services;

using Common.Core.Dtos;
using Extensions;
using Lib.Common.Web.Security;
using Lib.Data.Wallet;
using Lib.Data.Wallet.Enums;
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
public class WalletService : IWalletService
{
    private readonly WalletDbContext _walletDbContext;
    private readonly ICurrentUserService _currentUserService;
    private IConfiguration _configuration;
    private readonly ILogger<WalletService> _logger;
    private readonly IMapper _mapper;
    public WalletService(WalletDbContext walletDbContext
        , ICurrentUserService currentUserService
        , IConfiguration configuration
        , ILogger<WalletService> logger
        , IMapper mapper)
    {
        _walletDbContext = walletDbContext;
        _currentUserService = currentUserService;
        _configuration = configuration;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<int> GetTotalPurchaseOfChapterAsync(Guid chapterId, DateTime? date = null)
    {
        var query = _walletDbContext.WalletTransactions
            .Where(x => x.Type == TransactionType.BUY_CHAPTER
                        && x.Status == TransactionStatus.SUCCESS
                        && x.RelatedId == chapterId);

        if (date != null && date.HasValue)
        {
            var firstDate = date.Value.FirstDate();
            var lastDate = date.Value.LastDate();

            query = query.Where(x => x.CreatedDate >= firstDate && x.CreatedDate <= lastDate);
        }
        var transactions = await query.CountAsync();
        return transactions;
    }
    public async Task<List<ChapterPurchaseDto>> GetPurchaseOfChaptersAsync(List<Guid> chapterIds, DateTime? date = null)
    {
        var query = _walletDbContext.WalletTransactions
            .Where(x => x.Type == TransactionType.BUY_CHAPTER
                        && x.Status == TransactionStatus.SUCCESS
                        && x.RelatedId.HasValue
                        && chapterIds.Contains(x.RelatedId.Value));

        if (date != null && date.HasValue)
        {
            var firstDate = date.Value.FirstDate();
            var lastDate = date.Value.LastDate();

            query = query.Where(x => x.CreatedDate >= firstDate && x.CreatedDate <= lastDate);
        }
        var chapterPurchases = await query.GroupBy(x => x.RelatedId)
                                    .Select(n => new ChapterPurchaseDto
                                    {
                                        ChapterId = n.Key.Value,
                                        Purchases = n.Count()
                                    }).ToListAsync();

        return chapterPurchases;
    }
    public async Task<UserPurchaseData> GetRevenueSaleChapterOfUserAsync(Guid userId, DateTime? date = null)
    {
        var detailTrans = _walletDbContext.UserPurchaseTransactions
            .Where(x => x.IsPaid == false
                        && x.CreatorUserId == userId)
            .ToList();

        if (detailTrans == null || !detailTrans.Any())
        {
            return new UserPurchaseData()
            {
                UserId = userId,
                Amount = 0
            };
        }

        var transIds = detailTrans.Select(x => x.WalletTransactionId).ToList();

        var query = _walletDbContext.WalletTransactions
            .Where(x => transIds.Contains(x.Id)
                        && x.Type == TransactionType.BUY_CHAPTER
                        && x.Status == TransactionStatus.SUCCESS);

        if (date != null && date.HasValue)
        {
            var firstDate = date.Value.FirstDate();
            var lastDate = date.Value.LastDate();

            query = query.Where(x => x.CreatedDate >= firstDate && x.CreatedDate <= lastDate);
        }

        var purchaseAmount = await query.SumAsync(x => x.Amount);

        return new UserPurchaseData
        {
            UserId = userId,
            Amount = purchaseAmount
        };
    }
    public async Task<List<RevenueChartData>> GetRevenueSaleChapterByYearAsync(Guid userId, int year)
    {
        var beginOfYear = year.BeginOfYear();
        var endOfYear = year.EndOfYear();

        var detailTrans = _walletDbContext.UserPurchaseTransactions
            .Where(x => x.CreatorUserId == userId && x.CreatedDate >= beginOfYear && x.CreatedDate <= endOfYear)
            .ToList();

        if (detailTrans == null || !detailTrans.Any())
        {
            return new List<RevenueChartData>();
        }

        var transIds = detailTrans.Select(x => x.WalletTransactionId).ToList();

        var query = _walletDbContext.WalletTransactions
            .Where(x => transIds.Contains(x.Id)
                        && x.Type == TransactionType.BUY_CHAPTER
                        && x.Status == TransactionStatus.SUCCESS);

        var yearData = await query.GroupBy(x => new
        {
            Year = x.CreatedDate.Year,
            Month = x.CreatedDate.Month
        })
                                    .Select(n => new RevenueChartData
                                    {
                                        Year = n.Key.Year,
                                        Month = n.Key.Month,
                                        Amount = n.Sum(x => x.Amount)
                                    })
                                    .OrderBy(x => x.Year)
                                    .ThenBy(x => x.Month)
                                    .ToListAsync();

        return yearData;
    }
    public async Task<List<RevenueChartData>> GetRevenueSaleChapterByMonthAsync(Guid userId, int month)
    {
        var currentYear = DateTime.UtcNow.Year;
        var beginOfMonth = month.BeginOfMonth();
        var endOfMonth = month.EndOfMonth();

        var detailTrans = _walletDbContext.UserPurchaseTransactions
            .Where(x => x.CreatorUserId == userId && x.CreatedDate >= beginOfMonth && x.CreatedDate <= endOfMonth)
            .ToList();

        if (detailTrans == null || !detailTrans.Any())
        {
            return new List<RevenueChartData>();
        }

        var transIds = detailTrans.Select(x => x.WalletTransactionId).ToList();

        var query = _walletDbContext.WalletTransactions
            .Where(x => transIds.Contains(x.Id)
                        && x.Type == TransactionType.BUY_CHAPTER
                        && x.Status == TransactionStatus.SUCCESS);

        var monthData = await query.GroupBy(x => new
        {
            Year = x.CreatedDate.Year,
            Month = x.CreatedDate.Month,
            Day = x.CreatedDate.Day
        })
                                    .Select(n => new RevenueChartData
                                    {
                                        Year = n.Key.Year,
                                        Month = n.Key.Month,
                                        Day = n.Key.Day,
                                        Amount = n.Sum(x => x.Amount)
                                    })
                                    .OrderBy(x => x.Year)
                                    .ThenBy(x => x.Month)
                                    .ThenBy(x => x.Day)
                                    .ToListAsync();

        return monthData;
    }
    public async Task<List<CountChartData>> GetNumberOfSaleChapterByYearAsync(Guid userId, int year)
    {
        var beginOfYear = year.BeginOfYear();
        var endOfYear = year.EndOfYear();

        var detailTrans = _walletDbContext.UserPurchaseTransactions
            .Where(x => x.CreatorUserId == userId && x.CreatedDate >= beginOfYear && x.CreatedDate <= endOfYear)
            .ToList();

        if (detailTrans == null || !detailTrans.Any())
        {
            return new List<CountChartData>();
        }

        var transIds = detailTrans.Select(x => x.WalletTransactionId).ToList();

        var query = _walletDbContext.WalletTransactions
            .Where(x => transIds.Contains(x.Id)
                        && x.Type == TransactionType.BUY_CHAPTER
                        && x.Status == TransactionStatus.SUCCESS);

        var yearData = await query.GroupBy(x => new
        {
            Year = x.CreatedDate.Year,
            Month = x.CreatedDate.Month
        })
                                    .Select(n => new CountChartData
                                    {
                                        Year = n.Key.Year,
                                        Month = n.Key.Month,
                                        Count = n.Count()
                                    })
                                    .OrderBy(x => x.Year)
                                    .ThenBy(x => x.Month)
                                    .ToListAsync();

        return yearData;
    }
    public async Task<List<CountChartData>> GetNumberOfSaleChapterByMonthAsync(Guid userId, int month)
    {
        var currentYear = DateTime.UtcNow.Year;
        var beginOfMonth = month.BeginOfMonth();
        var endOfMonth = month.EndOfMonth();

        var detailTrans = _walletDbContext.UserPurchaseTransactions
            .Where(x => x.CreatorUserId == userId && x.CreatedDate >= beginOfMonth && x.CreatedDate <= endOfMonth)
            .ToList();

        if (detailTrans == null || !detailTrans.Any())
        {
            return new List<CountChartData>();
        }

        var transIds = detailTrans.Select(x => x.WalletTransactionId).ToList();

        var query = _walletDbContext.WalletTransactions
            .Where(x => transIds.Contains(x.Id)
                        && x.Type == TransactionType.BUY_CHAPTER
                        && x.Status == TransactionStatus.SUCCESS);

        var monthData = await query.GroupBy(x => new
        {
            Year = x.CreatedDate.Year,
            Month = x.CreatedDate.Month,
            Day = x.CreatedDate.Day
        })
                                    .Select(n => new CountChartData
                                    {
                                        Year = n.Key.Year,
                                        Month = n.Key.Month,
                                        Day = n.Key.Day,
                                        Count = n.Count()
                                    })
                                    .OrderBy(x => x.Year)
                                    .ThenBy(x => x.Month)
                                    .ThenBy(x => x.Day)
                                    .ToListAsync();

        return monthData;
    }
}
