using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Mcsg.Social.Api.Services;

using Common.Core.Dtos;
using Common.Core.Enums;
using Extensions;
using Interfaces;
using Lib.Common.Web.Security;
using Lib.Data.Wallet;
using Lib.Data.Wallet.Enums;
using Models.Earning;

public class WalletService : IWalletService
{
    private readonly WalletContext _walletDbContext;
    private readonly ICurrentUserService _currentUserService;
    private IConfiguration _configuration;
    private readonly ILogger<WalletService> _logger;
    private readonly IMapper _mapper;

    public WalletService(WalletContext walletDbContext
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
                        && x.Status == TransactionStatus.Success
                        && x.RelatedId == chapterId);

        if (date != null && date.HasValue)
        {
            var firstDate = date.Value.FirstDate();
            var lastDate = date.Value.LastDate();

            query = query.Where(x => x.CreatedOn >= firstDate && x.CreatedOn <= lastDate);
        }
        var transactions = await query.CountAsync();
        return transactions;
    }
    public async Task<List<ChapterPurchaseDto>> GetPurchaseOfChaptersAsync(List<Guid> chapterIds, DateTime? date = null)
    {
        var query = _walletDbContext.WalletTransactions
            .Where(x => x.Type == TransactionType.BUY_CHAPTER
                        && x.Status == TransactionStatus.Success
                        && x.RelatedId.HasValue
                        && chapterIds.Contains(x.RelatedId.Value));

        if (date != null && date.HasValue)
        {
            var firstDate = date.Value.FirstDate();
            var lastDate = date.Value.LastDate();

            query = query.Where(x => x.CreatedOn >= firstDate && x.CreatedOn <= lastDate);
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
                        && x.Status == TransactionStatus.Success);

        if (date != null && date.HasValue)
        {
            var firstDate = date.Value.FirstDate();
            var lastDate = date.Value.LastDate();

            query = query.Where(x => x.CreatedOn >= firstDate && x.CreatedOn <= lastDate);
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
            .Where(x => x.CreatorUserId == userId && x.CreatedOn >= beginOfYear && x.CreatedOn <= endOfYear)
            .ToList();

        if (detailTrans == null || !detailTrans.Any())
        {
            return new List<RevenueChartData>();
        }

        var transIds = detailTrans.Select(x => x.WalletTransactionId).ToList();

        var query = _walletDbContext.WalletTransactions
            .Where(x => transIds.Contains(x.Id)
                        && x.Type == TransactionType.BUY_CHAPTER
                        && x.Status == TransactionStatus.Success);

        var yearData = await query.GroupBy(x => new
        {
            Year = x.CreatedOn.Year,
            Month = x.CreatedOn.Month
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
            .Where(x => x.CreatorUserId == userId && x.CreatedOn >= beginOfMonth && x.CreatedOn <= endOfMonth)
            .ToList();

        if (detailTrans == null || !detailTrans.Any())
        {
            return new List<RevenueChartData>();
        }

        var transIds = detailTrans.Select(x => x.WalletTransactionId).ToList();

        var query = _walletDbContext.WalletTransactions
            .Where(x => transIds.Contains(x.Id)
                        && x.Type == TransactionType.BUY_CHAPTER
                        && x.Status == TransactionStatus.Success);

        var monthData = await query.GroupBy(x => new
        {
            Year = x.CreatedOn.Year,
            Month = x.CreatedOn.Month,
            Day = x.CreatedOn.Day
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
            .Where(x => x.CreatorUserId == userId && x.CreatedOn >= beginOfYear && x.CreatedOn <= endOfYear)
            .ToList();

        if (detailTrans == null || !detailTrans.Any())
        {
            return new List<CountChartData>();
        }

        var transIds = detailTrans.Select(x => x.WalletTransactionId).ToList();

        var query = _walletDbContext.WalletTransactions
            .Where(x => transIds.Contains(x.Id)
                        && x.Type == TransactionType.BUY_CHAPTER
                        && x.Status == TransactionStatus.Success);

        var yearData = await query.GroupBy(x => new
        {
            Year = x.CreatedOn.Year,
            Month = x.CreatedOn.Month
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
            .Where(x => x.CreatorUserId == userId && x.CreatedOn >= beginOfMonth && x.CreatedOn <= endOfMonth)
            .ToList();

        if (detailTrans == null || !detailTrans.Any())
        {
            return new List<CountChartData>();
        }

        var transIds = detailTrans.Select(x => x.WalletTransactionId).ToList();

        var query = _walletDbContext.WalletTransactions
            .Where(x => transIds.Contains(x.Id)
                        && x.Type == TransactionType.BUY_CHAPTER
                        && x.Status == TransactionStatus.Success);

        var monthData = await query.GroupBy(x => new
        {
            Year = x.CreatedOn.Year,
            Month = x.CreatedOn.Month,
            Day = x.CreatedOn.Day
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
