using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Mcsg.Comic.Api.Services;

using Common.Core.Dtos;
using Common.Core.Enums;
using Common.SeedWork.Exceptions;
using Constants;
using Extensions;
using Interfaces;
using Lib.Common.Extensions;
using Lib.Common.Web.Security;
using Lib.Data.Wallet;
using Lib.Data.Wallet.Enums;
using Models.Earning;

public class AffiliateService : IAffiliateService
{
    private readonly WalletDbContext _walletDbContext;
    private readonly ICurrentUserService _currentUserService;
    private IConfiguration _configuration;
    private readonly ILogger<WalletService> _logger;
    private readonly IMapper _mapper;

    public AffiliateService(WalletDbContext walletDbContext
        , ICurrentUserService currentUserService
        , IConfiguration configuration
        , ILogger<WalletService> logger
        , IMapper mapper
        , ISetting setting)
    {
        _walletDbContext = walletDbContext;
        _currentUserService = currentUserService;
        _configuration = configuration;
        _logger = logger;
        _mapper = mapper;
        _setting = setting;
    }
    public async Task<EarningDataModel> GetSaleAffiliateAsync(Guid userId, DateTime? date = null)
    {
        var affiliateTrans = _walletDbContext.UserPurchaseTransactions
            .Where(x => x.IsPaid == false
                        && x.AffiliateUserId == userId)
            .ToList();

        if (affiliateTrans == null || !affiliateTrans.Any())
        {
            return new EarningDataModel()
            {
                Type = Enums.EarningDataType.SaleAffiliate,
                Amount = 0,
            };
        }

        var transIds = affiliateTrans.Select(x => x.WalletTransactionId).ToList();

        var query = _walletDbContext.WalletTransactions
            .Where(x => transIds.Contains(x.Id)
                        && x.Status == TransactionStatus.SUCCESS);

        if (date != null && date.HasValue)
        {
            var firstDate = date.Value.FirstDate();
            var lastDate = date.Value.LastDate();

            query = query.Where(x => x.CreatedOn >= firstDate && x.CreatedOn <= lastDate);
        }

        var affiliateAmount = await query.SumAsync(x => x.Amount);

        return new EarningDataModel()
        {
            Type = Enums.EarningDataType.SaleAffiliate,
            Amount = affiliateAmount,
        };
    }
    public string GetAffiliateCode(Guid userId, EarningAffiliateCodeR req)
    {
        var entityTypes = Enum.GetNames(typeof(AffiliateEntityType)).ToList().ConvertAll(x => x.ToLower());

        if (!entityTypes.Contains(req.Type.ToLower()))
        {
            throw new BadRequestException(ApiErrorCode.INVALID_AFFILIATE_ENTITY_TYPE, ApiErrorMessage.INVALID_AFFILIATE_ENTITY_TYPE);
        }

        string affiliateCode = "";
        var data = new AffiliateDto()
        {
            AffiliateUserId = userId,
            EntityHashId = req.HashId,
            EntityType = (AffiliateEntityType)Enum.Parse(typeof(AffiliateEntityType), req.Type, true)
        };

        affiliateCode = data.ToAffiliateCode(_setting.EncryptKey);

        return affiliateCode;
    }
    public async Task<List<RevenueChartData>> GetRevenueAffiliateByYearAsync(Guid userId, int year)
    {
        var beginOfYear = year.BeginOfYear();
        var endOfYear = year.EndOfYear();

        var detailTrans = _walletDbContext.UserPurchaseTransactions
            .Where(x => x.AffiliateUserId == userId && x.CreatedOn >= beginOfYear && x.CreatedOn <= endOfYear)
            .ToList();

        if (detailTrans == null || !detailTrans.Any())
        {
            return new List<RevenueChartData>();
        }

        var transIds = detailTrans.Select(x => x.WalletTransactionId).ToList();

        var affiliateTransType = new List<TransactionType>() { TransactionType.BUY_PREMIUM
                                                , TransactionType.BUY_CHAPTER
                                                , TransactionType.BUY_SERIES };

        var query = _walletDbContext.WalletTransactions
            .Where(x => transIds.Contains(x.Id)
                        && affiliateTransType.Contains(x.Type)
                        && x.Status == TransactionStatus.SUCCESS);

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
    public async Task<List<RevenueChartData>> GetRevenueAffiliateByMonthAsync(Guid userId, int month)
    {
        var currentYear = DateTime.UtcNow.Year;
        var beginOfMonth = month.BeginOfMonth();
        var endOfMonth = month.EndOfMonth();

        var detailTrans = _walletDbContext.UserPurchaseTransactions
            .Where(x => x.AffiliateUserId == userId && x.CreatedOn >= beginOfMonth && x.CreatedOn <= endOfMonth)
            .ToList();

        if (detailTrans == null || !detailTrans.Any())
        {
            return new List<RevenueChartData>();
        }

        var transIds = detailTrans.Select(x => x.WalletTransactionId).ToList();

        var affiliateTransType = new List<TransactionType>() { TransactionType.BUY_PREMIUM
                                                , TransactionType.BUY_CHAPTER
                                                , TransactionType.BUY_SERIES };

        var query = _walletDbContext.WalletTransactions
            .Where(x => transIds.Contains(x.Id)
                        && affiliateTransType.Contains(x.Type)
                        && x.Status == TransactionStatus.SUCCESS);

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
    public async Task<List<CountChartData>> GetNumberOfAffiliateByYearAsync(Guid userId, int year)
    {
        var beginOfYear = year.BeginOfYear();
        var endOfYear = year.EndOfYear();

        var detailTrans = _walletDbContext.UserPurchaseTransactions
            .Where(x => x.AffiliateUserId == userId && x.CreatedOn >= beginOfYear && x.CreatedOn <= endOfYear)
            .ToList();

        if (detailTrans == null || !detailTrans.Any())
        {
            return new List<CountChartData>();
        }

        var transIds = detailTrans.Select(x => x.WalletTransactionId).ToList();

        var affiliateTransType = new List<TransactionType>() { TransactionType.BUY_PREMIUM
                                                , TransactionType.BUY_CHAPTER
                                                , TransactionType.BUY_SERIES };

        var query = _walletDbContext.WalletTransactions
            .Where(x => transIds.Contains(x.Id)
                        && affiliateTransType.Contains(x.Type)
                        && x.Status == TransactionStatus.SUCCESS);

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
    public async Task<List<CountChartData>> GetNumberOfAffiliateByMonthAsync(Guid userId, int month)
    {
        var currentYear = DateTime.UtcNow.Year;
        var beginOfMonth = month.BeginOfMonth();
        var endOfMonth = month.EndOfMonth();

        var detailTrans = _walletDbContext.UserPurchaseTransactions
            .Where(x => x.AffiliateUserId == userId && x.CreatedOn >= beginOfMonth && x.CreatedOn <= endOfMonth)
            .ToList();

        if (detailTrans == null || !detailTrans.Any())
        {
            return new List<CountChartData>();
        }

        var transIds = detailTrans.Select(x => x.WalletTransactionId).ToList();

        var affiliateTransType = new List<TransactionType>() { TransactionType.BUY_PREMIUM
                                                , TransactionType.BUY_CHAPTER
                                                , TransactionType.BUY_SERIES };

        var query = _walletDbContext.WalletTransactions
            .Where(x => transIds.Contains(x.Id)
                        && affiliateTransType.Contains(x.Type)
                        && x.Status == TransactionStatus.SUCCESS);

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

    #region -- Fields --

    /// <summary>
    /// Setting
    /// </summary>
    private readonly ISetting _setting;

    #endregion
}
