using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using Microsoft.EntityFrameworkCore;

namespace Mcsg.Wallet.Api.Services;

using Common.Core.Distributor;
using Common.Core.Enums;
using Common.Core.Extensions;
using Common.Domain.Entities;
using Common.SeedWork.Exceptions;
using Common.SeedWork.Extensions;
using Constants;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Identity.Api.Protos;
using Interfaces;
using Lib.Common.Extensions;
using Lib.Common.Models;
using Lib.Common.Web.Security;
using Lib.Data.Repositories;
using Lib.Data.Repositories.Interface;
using Models;
using Requests;
using static Common.Core.Constants.Setting;

public partial class PremiumService : BaseSettingS, IPremiumService
{
    private readonly IConfiguration _configuration;
    private readonly IWalletContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly DistributeManager _distributeManager;
    private readonly IBankService _bankService;
    private readonly IRepository<User> _userRepository;

    public PremiumService(IConfiguration configuration,
        DistributeManager distributeManager,
        IBankService bankService,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IWalletContext walletDbContext, ISetting setting) : base(walletDbContext, setting)
    {
        _configuration = configuration;
        _dbContext = walletDbContext;
        _bankService = bankService;
        _currentUserService = currentUserService;
        _distributeManager = distributeManager;
        _userRepository = unitOfWork.GetRepository<User>();
    }
    #region Premium package
    public async Task<IEnumerable<PremiumPackageResponse>> GetPremiumPackage()
    {
        var packageResponses = await _dbContext.PremiumPackages
            .AsNoTracking()
            .Select(x => new PremiumPackageResponse
            {
                No = x.No,
                Description = x.Description,
                LiveTimeDay = x.LiveTimeDay,
                Name = x.Name,
                Price = x.Price,
                PricePerMonth = x.PricePerMonth,
                DiscountPercent = x.FirstTimeDiscountPercent,
                DiscountPrice = x.FirstTimePrice,
                DiscountPricePerMonth = x.FirstTimePricePerMonth,
                IsPackage = x.IsPackage
            })
            .ToListAsync();

        return packageResponses;
    }

    public async Task<bool> BuyPremium(PremiumBuyPremiumR req)
    {
        var userId = _currentUserService?.Session?.UserId;
        var userWallet = await _dbContext.UserWallets.Where(x => x.UserId == userId).FirstOrDefaultAsync();
        var package = await _dbContext.PremiumPackages.Where(x => x.No == req.PremiumPackageNo).FirstOrDefaultAsync();
        var now = DateTime.UtcNow;

        if (userWallet == null)
        {
            throw new BadRequestException(ApiErrorCodes.USER_NOT_FOUND, ApiErrorMessage.USER_NOT_FOUND);
        }
        if (package == null)
        {
            throw new BadRequestException(ApiErrorCodes.PACKAGE_NOT_FOUND, ApiErrorMessage.PACKAGE_NOT_FOUND);
        }

        if ((userWallet.Point + userWallet.RewardPoint) < package?.FirstTimePrice)
        {
            throw new BadRequestException(ApiErrorCodes.BALANCE_NOT_ENOUGH, ApiErrorMessage.BALANCE_NOT_ENOUGH);
        }

        //CHECK DUPLICATE PURCHASE
        var minuteDate = now.AddSeconds(-30);
        var checkPremium = await _dbContext.UserPremiumPackages.Where(x => x.UserWalletId == userWallet.Id
            && !x.IsDelete
            && x.PremiumPackageNo == package.No
            && x.CreatedOn > minuteDate
        ).AnyAsync();
        if (checkPremium)
        {
            throw new BadRequestException(ApiErrorCodes.ALREADY_PURCHARED, ApiErrorMessage.ALREADY_PURCHARED);
        }

        var transaction = new WalletTransaction
        {
            CreatedOn = now,
            CreatedBy = _currentUserService?.Session?.UserId,
            Id = Guid.NewGuid(),
            Amount = package.FirstTimePrice,
            IsFromSystem = false,
            Content = "BUY PACKAGE: " + package.Name,
            SystemMessage = "BUY PACKAGE: " + package.No,
            ReferenceNumber = Default.ReferenceNumberLength.GetRandomString().ToLower(),
            ModifiedOn = now,
            SourceUserWalletId = userWallet.Id,
            Status = TransactionStatus.Pending,
            Type = TransactionType.BuyPremium
        };
        var purchaseHistory = new UserPurchaseTransaction
        {
            WalletTransactionId = transaction.Id,
            Paymethod = _bankService.GetPayMethodTitle(req.PayMethodName)
        };

        if (!string.IsNullOrEmpty(req.AffiliateCode))
        {
            string encryptKey = _setting.EncryptKey;
            var affiliateData = req.AffiliateCode.ToAffiliateData(encryptKey);
            if (affiliateData != null)
            {
                purchaseHistory.AffiliateUserId = affiliateData.AffiliateUserId;
            }
        }

        await _dbContext.UserPurchaseTransactions.AddAsync(purchaseHistory);
        await _dbContext.WalletTransactions.AddAsync(transaction);
        await _dbContext.SaveChangesAsync(default);
        await SyncBuyPremium(userWallet, package, transaction);

        return true;
    }
    private async Task SyncBuyPremium(UserWallet userWallet, PremiumPackage package, WalletTransaction transaction)
    {
        if (userWallet != null)
        {
            if ((userWallet.Point + userWallet.RewardPoint) < transaction.Amount)
            {
                transaction.Status = TransactionStatus.Failed;
                transaction.Content = transaction.Content + " không đủ point";
                _dbContext.WalletTransactions.Update(transaction);
                //await LogError(buyPremiumData.TransactionId, "Không đủ tiền");
                return;//ko đủ số dư
            }
            transaction.Status = TransactionStatus.Success;
            var nowDate = DateTime.UtcNow.Date;
            //Select other userPackage
            var lastPackage = await _dbContext.UserPremiumPackages.Where(x => x.UserWalletId == userWallet.Id).OrderByDescending(x => x.EndDate).FirstOrDefaultAsync();
            var endDate = nowDate.AddDays(package.LiveTimeDay);
            var startDate = nowDate;
            if (lastPackage != null && lastPackage.EndDate >= nowDate)
            {
                startDate = endDate.AddDays(1);
                endDate = lastPackage.EndDate.AddDays(package.LiveTimeDay);
            }
            var userPremium = new UserPremiumPackage
            {
                PremiumPackageNo = package.No,
                UserWalletId = userWallet.Id,
                StartDate = nowDate,
                EndDate = endDate,
                CreatedBy = userWallet.UserId,
                CreatedOn = DateTime.UtcNow
            };
            //Transaction purchase history 
            await _dbContext.UserPremiumPackages.AddAsync(userPremium);
            //Remove point
            if (userWallet.RewardPoint >= transaction.Amount)
            {
                userWallet.RewardPoint -= transaction.Amount;
            }
            else
            {
                var remainingAmount = transaction.Amount - userWallet.RewardPoint;
                userWallet.RewardPoint = 0;
                userWallet.Point -= remainingAmount;
            }
            transaction.IsConfirmed = true;
            _dbContext.UserWallets.Update(userWallet);
            _dbContext.WalletTransactions.Update(transaction);

            await UpdatePremiumDate(userWallet.UserId, userPremium.EndDate);

            await _dbContext.SaveChangesAsync(default);
        }
        else
        {
            //await LogError(buyPremiumData.TransactionId, $"Lỗi user id {userId}");
        }
    }
    public async Task<BuyItemResp> SelectPremiumPackage(int? packageNo)
    {
        var result = new BuyItemResp();
        var userId = _currentUserService?.Session?.UserId;
        var userWallet = await _dbContext.UserWallets.Where(x => x.UserId == userId).FirstOrDefaultAsync();

        result.PayMethods = _bankService.GetPayMethods();
        result.CurrencyTypes = _bankService.GetCurrencyTypeRatios();
        result.CurrentPoint = userWallet != null ? (userWallet.Point + userWallet.RewardPoint) : 0;

        if (packageNo != null)
        {
            var package = await _dbContext.PremiumPackages.Where(x => x.No == packageNo).FirstOrDefaultAsync();
            result.Item = new ItemSeletedResp
            {
                Name = package.Name,
                Price = package.Price
            };
            if (result.CurrentPoint >= package.Price)
            {
                result.PayMethods.ForEach(x =>
                {
                    if (x.Name != PayMethods.POINT)
                    {
                        x.IsEnable = false;
                    }
                });
            }
        }

        return result;
    }
    #endregion

    #region Chapter
    public async Task<bool> BuyChapter(PremiumBuyChapterR req)
    {
        var userId = _currentUserService?.Session?.UserId;
        var userWallet = await _dbContext.UserWallets
            .Include(x => x.SourceUserWalletTransactions.Where(y => y.SourceUserWallet.UserId == userId && y.RelatedId == req.ChapterId))
            .Where(x => x.UserId == userId).FirstOrDefaultAsync();

        var now = DateTime.UtcNow;

        if (userWallet == null)
        {
            throw new BadRequestException(ApiErrorCodes.USER_NOT_FOUND, ApiErrorMessage.USER_NOT_FOUND);
        }

        if ((userWallet.Point + userWallet.RewardPoint) < Default.ChapterPrice)
        {
            throw new BadRequestException(ApiErrorCodes.BALANCE_NOT_ENOUGH, ApiErrorMessage.BALANCE_NOT_ENOUGH);
        }

        if (userWallet.SourceUserWalletTransactions.Count > 0 && userWallet.SourceUserWalletTransactions.Any(x => x.RelatedId == req.ChapterId))
        {
            throw new BadRequestException(ApiErrorCodes.ALREADY_PURCHARED, ApiErrorMessage.ALREADY_PURCHARED);
        }

        var transaction = new WalletTransaction
        {
            CreatedOn = DateTime.UtcNow,
            CreatedBy = _currentUserService?.Session?.UserId,
            Id = Guid.NewGuid(),
            Amount = Default.ChapterPrice,
            IsFromSystem = false,
            Content = "BUY CHAPTER ID: " + req.ChapterId,
            SystemMessage = "BUY CHAPTER ID: " + req.ChapterId,
            ReferenceNumber = Default.ReferenceNumberLength.GetRandomString().ToLower(),
            ModifiedOn = DateTime.UtcNow,
            SourceUserWalletId = userWallet.Id,
            RelatedId = req.ChapterId,
            Status = TransactionStatus.Pending,
            Type = TransactionType.BuyChapter
        };

        var purchaseHistory = new UserPurchaseTransaction
        {
            WalletTransactionId = transaction.Id,
            Paymethod = _bankService.GetPayMethodTitle(req.PayMethodName),
            IsPaid = false // Job should be create transaction=> type : Platform transfer.=> transfer to creator or affilater. 
            //Job should be set IsPaid = true when job transfer money from platform to cretor or affilater
        };


        if (!string.IsNullOrEmpty(req.AffiliateCode))
        {
            string encryptKey = _setting.EncryptKey;
            var affiliateData = req.AffiliateCode.ToAffiliateData(encryptKey);
            if (affiliateData != null)
            {
                purchaseHistory.AffiliateUserId = affiliateData.AffiliateUserId;
            }
        }

        await _dbContext.UserPurchaseTransactions.AddAsync(purchaseHistory);
        await _dbContext.WalletTransactions.AddAsync(transaction);
        await SyncBuyChapter(userId ?? Guid.Empty, transaction.Id);
        await _dbContext.SaveChangesAsync(default);

        return true;
    }
    private async Task SyncBuyChapter(Guid userId, Guid TransactionId)
    {
        //sync wallet profilename
        await _distributeManager.Deliver(new SyncDataDistributeItem
        {
            Data = new SyncData
            {
                TargetDb = SyncTargetDb.WALLETDB,
                TargetEntity = SyncTargetEntity.WALLET_USER_BUY_CHAPTER,
                Data = new Dictionary<object, object>
                {
                    {
                        userId, new BuyItemData {
                        TransactionId = TransactionId
                    } }
                }
            }
        });
    }
    public async Task<BuyItemResp> SelectChapterPackage(Guid chapterId)
    {
        var result = new BuyItemResp();
        var userId = _currentUserService?.Session?.UserId;
        var userWallet = await _dbContext.UserWallets.Where(x => x.UserId == userId).FirstOrDefaultAsync();
        result.PayMethods = _bankService.GetDepositMethods();
        result.CurrencyTypes = _bankService.GetCurrencyTypeRatios();
        result.CurrentPoint = userWallet != null ? (userWallet.Point + userWallet.RewardPoint) : 0;
        result.Item = new ItemSeletedResp
        {
            Name = "Buy chapter",
            Price = Default.ChapterPrice
        };
        return result;
    }
    #endregion

    #region Serie
    public async Task<bool> BuySerieAsync(PremiumBuySerieR req)
    {
        var userId = _currentUserService?.Session?.UserId;
        var userWallet = await _dbContext.UserWallets.Where(x => x.UserId == userId).FirstOrDefaultAsync();

        var now = DateTime.UtcNow;

        if (userWallet == null)
        {
            throw new BadRequestException(ApiErrorCodes.USER_NOT_FOUND, ApiErrorMessage.USER_NOT_FOUND);
        }

        if ((userWallet.Point + userWallet.RewardPoint) < Default.ChapterPrice)
        {
            throw new BadRequestException(ApiErrorCodes.BALANCE_NOT_ENOUGH, ApiErrorMessage.BALANCE_NOT_ENOUGH);
        }

        var transaction = new WalletTransaction
        {
            CreatedOn = DateTime.UtcNow,
            CreatedBy = _currentUserService?.Session?.UserId,
            Id = Guid.NewGuid(),
            IsFromSystem = false,
            Content = "BUY SERIES ID: " + req.SerieId,
            SystemMessage = "BUY SERIES ID: " + req.SerieId,
            ReferenceNumber = Default.ReferenceNumberLength.GetRandomString().ToLower(),
            ModifiedOn = DateTime.UtcNow,
            SourceUserWalletId = userWallet.Id,
            RelatedId = req.SerieId,
            Status = TransactionStatus.Pending,
            Type = TransactionType.BuySeries
        };

        var purchaseHistory = new UserPurchaseTransaction
        {
            WalletTransactionId = transaction.Id,
            Paymethod = _bankService.GetPayMethodTitle(req.PayMethodName),
            IsPaid = false // Job should be create transaction=> type : Platform transfer.=> transfer to creator or affilater. 
            //Job should be set IsPaid = true when job transfer money from platform to cretor or affilater
        };

        if (!string.IsNullOrEmpty(req.AffiliateCode))
        {
            string encryptKey = _setting.EncryptKey;
            var affiliateData = req.AffiliateCode.ToAffiliateData(encryptKey);
            if (affiliateData != null)
            {
                purchaseHistory.AffiliateUserId = affiliateData.AffiliateUserId;
            }
        }

        await _dbContext.UserPurchaseTransactions.AddAsync(purchaseHistory);
        await _dbContext.WalletTransactions.AddAsync(transaction);
        await SyncBuySerieAsync(userId ?? Guid.Empty, transaction.Id);
        await _dbContext.SaveChangesAsync(default);

        return true;
    }
    private async Task SyncBuySerieAsync(Guid userId, Guid TransactionId)
    {
        //sync wallet profilename
        await _distributeManager.Deliver(new SyncDataDistributeItem
        {
            Data = new SyncData
            {
                TargetDb = SyncTargetDb.WALLETDB,
                TargetEntity = SyncTargetEntity.WALLET_USER_BUY_SERIES,
                Data = new Dictionary<object, object>
                {
                    {
                        userId, new BuyItemData {
                        TransactionId = TransactionId
                    } }
                }
            }
        });
    }
    #endregion

    /// <summary>
    /// UpdatePremiumDate
    /// </summary>
    /// <param name="userId">UserId</param>
    /// <param name="premiumDate">PremiumDate</param>
    /// <returns>Return the result</returns>
    private async Task<BaseRsp> UpdatePremiumDate(Guid userId, DateTime premiumDate)
    {
        var res = new BaseRsp { Success = true };

        try
        {
            using var channel = GrpcChannel.ForAddress(_setting.Rpc.Web.Identity!);

            var client = new UserProto.UserProtoClient(channel);
            var request = new UserUpdateReq
            {
                UserUid = userId.ToString(),
                PremiumDate = Timestamp.FromDateTime(premiumDate)
            };
            var rsp = await client.UpdateAsync(request);
            res.Id = rsp.Id;
        }
        catch (Exception ex)
        {
            res.Message = ex.Message;
            ex.Message.LogError();
        }

        return res;
    }
}
