using Dapper;
using Mcsg.Lib.Common.Distributor;
using Mcsg.Lib.Common.Exceptions;
using Mcsg.Lib.Common.Extensions;
using Mcsg.Lib.Common.Models;
using Mcsg.Lib.Common.Web.Security;
using Mcsg.Lib.Data.Domain.Entities;
using Mcsg.Lib.Data.Repositories;
using Mcsg.Lib.Data.Repositories.Interface;
using Mcsg.Lib.Data.Wallet;
using Mcsg.Lib.Data.Wallet.Entities;
using Mcsg.Lib.Data.Wallet.Enums;
using Mcsg.Lib.Model.Const;
using Mcsg.Wallet.Api.Constants;
using Mcsg.Wallet.Api.Helpers;
using Mcsg.Wallet.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Mcsg.Wallet.Api.Services
{
    public interface IPremiumService
    {
        Task<IEnumerable<PremiumPackageResponse>> GetPremiumPackage();
        Task<BuyItemResp> SelectPremiumPackage(int? packageNo);
        Task<bool> BuyPremium(BuyPremiumReq req);
        Task<BuyItemResp> SelectChapterPackage(Guid chapterId);
        Task<bool> BuyChapter(BuyChapterReq req);
        Task<bool> BuySerieAsync(BuySerieReq req);
    }
    public partial class PremiumService : IPremiumService
    {
        private readonly IConfiguration _configuration;
        private readonly WalletDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;
        private readonly DistributeManager _distributeManager;
        private readonly IBankService _bankService;
        private readonly IRepository<Lib.Data.Domain.Entities.User> _userRepository;
        public PremiumService(IConfiguration configuration,
             DistributeManager distributeManager,
              IBankService bankService,
               IUnitOfWork unitOfWork,
             ICurrentUserService currentUserService,
            WalletDbContext walletDbContext)
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
        public async Task<bool> BuyPremium(BuyPremiumReq req)
        {
            var userId = _currentUserService?.Session?.UserId;
            var userWallet = await _dbContext.UserWallets.Where(x => x.UserId == userId).FirstOrDefaultAsync();
            var package = await _dbContext.PremiumPackages.Where(x => x.No == req.PremiumPackageNo).FirstOrDefaultAsync();
            var now = DateTime.UtcNow;

            if (userWallet == null)
            {
                throw new BadRequestException(ApiErrorCodes.USER_NOT_FOUND, ApiErrorMessage.USER_NOT_FOUND);
            }

            if ((userWallet.Point + userWallet.RewardPoint) < package.FirstTimePrice)
            {
                throw new BadRequestException(ApiErrorCodes.BALANCE_NOT_ENOUGH, ApiErrorMessage.BALANCE_NOT_ENOUGH);
            }

            //CHECK DUPLICATE PURCHASE
            var minuteDate = now.AddSeconds(-30);
            var checkPremium = await _dbContext.UserPremiumPackages.Where(x => x.UserWalletId == userWallet.Id
                && !x.IsDelete
                && x.PremiumPackageNo == package.No
                && x.CreatedDate > minuteDate
            ).AnyAsync();
            if (checkPremium)
            {
                throw new BadRequestException(ApiErrorCodes.ALREADY_PURCHARED, ApiErrorMessage.ALREADY_PURCHARED);
            }

            var transaction = new WalletTransaction
            {
                CreatedDate = now,
                CreatedBy = _currentUserService?.Session?.UserId,
                Id = Guid.NewGuid(),
                Amount = package.FirstTimePrice,
                IsFromSystem = false,
                Content = "BUY PACKAGE: " + package.Name,
                SystemMessage = "BUY PACKAGE: " + package.No,
                ReferenceNumber = StringHelper.GetRandomString(SystemConfig.ReferenceNumberLength).ToLower(),
                ModifiedDate = now,
                SourceUserWalletId = userWallet.Id,
                Status = TransactionStatus.PENDING,
                Type = TransactionType.BUY_PREMIUM
            };
            var purchaseHistory = new UserPurchaseTransaction
            {
                WalletTransactionId = transaction.Id,
                Paymethod = _bankService.GetPayMethodTitle(req.PayMethodName)
            };

            if (!string.IsNullOrEmpty(req.AffiliateCode))
            {
                string encryptKey = _configuration[SystemSettings.CONST_PAYMENT_ENCRYPTION_KEY];
                var affiliateData = req.AffiliateCode.ToAffiliateData(encryptKey);
                if (affiliateData != null)
                {
                    purchaseHistory.AffiliateUserId = affiliateData.AffiliateUserId;
                }
            }

            await _dbContext.UserPurchaseTransactions.AddAsync(purchaseHistory);
            await _dbContext.WalletTransactions.AddAsync(transaction);
            await _dbContext.SaveChangesAsync();
            await SyncBuyPremium(userWallet, package, transaction);

            return true;
        }
        private async Task SyncBuyPremium(UserWallet userWallet, PremiumPackage package, WalletTransaction transaction)
        {
            if (userWallet != null)
            {
                if ((userWallet.Point + userWallet.RewardPoint) < transaction.Amount)
                {
                    transaction.Status = TransactionStatus.FAILED;
                    transaction.Content = transaction.Content + " không đủ point";
                    _dbContext.WalletTransactions.Update(transaction);
                    //await LogError(buyPremiumData.TransactionId, "Không đủ tiền");
                    return;//ko đủ số dư
                }
                transaction.Status = TransactionStatus.SUCCESS;
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
                    CreatedDate = DateTime.UtcNow
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
                await _userRepository.Connection.QueryAsync(UpdatePremiumDate, new
                {
                    PremiumDate = DateOnly.FromDateTime(userPremium.EndDate),
                    UserId = userWallet.UserId,
                    DateTimeNow = nowDate
                });
                try
                {
                    await _dbContext.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    //await LogError(buyPremiumData.TransactionId, ex.Message);
                }
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
        public async Task<bool> BuyChapter(BuyChapterReq req)
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

            if ((userWallet.Point + userWallet.RewardPoint) < GlobalSystemConfig.ChapterPrice)
            {
                throw new BadRequestException(ApiErrorCodes.BALANCE_NOT_ENOUGH, ApiErrorMessage.BALANCE_NOT_ENOUGH);
            }

            if (userWallet.SourceUserWalletTransactions.Count > 0 && userWallet.SourceUserWalletTransactions.Any(x => x.RelatedId == req.ChapterId))
            {
                throw new BadRequestException(ApiErrorCodes.ALREADY_PURCHARED, ApiErrorMessage.ALREADY_PURCHARED);
            }

            var transaction = new WalletTransaction
            {
                CreatedDate = DateTime.UtcNow,
                CreatedBy = _currentUserService?.Session?.UserId,
                Id = Guid.NewGuid(),
                Amount = GlobalSystemConfig.ChapterPrice,
                IsFromSystem = false,
                Content = "BUY CHAPTER ID: " + req.ChapterId,
                SystemMessage = "BUY CHAPTER ID: " + req.ChapterId,
                ReferenceNumber = StringHelper.GetRandomString(SystemConfig.ReferenceNumberLength).ToLower(),
                ModifiedDate = DateTime.UtcNow,
                SourceUserWalletId = userWallet.Id,
                RelatedId = req.ChapterId,
                Status = TransactionStatus.PENDING,
                Type = TransactionType.BUY_CHAPTER
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
                string encryptKey = _configuration[SystemSettings.CONST_PAYMENT_ENCRYPTION_KEY];
                var affiliateData = req.AffiliateCode.ToAffiliateData(encryptKey);
                if (affiliateData != null)
                {
                    purchaseHistory.AffiliateUserId = affiliateData.AffiliateUserId;
                }
            }

            await _dbContext.UserPurchaseTransactions.AddAsync(purchaseHistory);
            await _dbContext.WalletTransactions.AddAsync(transaction);
            await SyncBuyChapter(userId ?? Guid.Empty, transaction.Id);
            await _dbContext.SaveChangesAsync();

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
                Price = GlobalSystemConfig.ChapterPrice
            };
            return result;
        }
        #endregion

        #region Serie
        public async Task<bool> BuySerieAsync(BuySerieReq req)
        {
            var userId = _currentUserService?.Session?.UserId;
            var userWallet = await _dbContext.UserWallets.Where(x => x.UserId == userId).FirstOrDefaultAsync();

            var now = DateTime.UtcNow;

            if (userWallet == null)
            {
                throw new BadRequestException(ApiErrorCodes.USER_NOT_FOUND, ApiErrorMessage.USER_NOT_FOUND);
            }

            if ((userWallet.Point + userWallet.RewardPoint) < GlobalSystemConfig.ChapterPrice)
            {
                throw new BadRequestException(ApiErrorCodes.BALANCE_NOT_ENOUGH, ApiErrorMessage.BALANCE_NOT_ENOUGH);
            }

            var transaction = new WalletTransaction
            {
                CreatedDate = DateTime.UtcNow,
                CreatedBy = _currentUserService?.Session?.UserId,
                Id = Guid.NewGuid(),
                IsFromSystem = false,
                Content = "BUY SERIES ID: " + req.SerieId,
                SystemMessage = "BUY SERIES ID: " + req.SerieId,
                ReferenceNumber = StringHelper.GetRandomString(SystemConfig.ReferenceNumberLength).ToLower(),
                ModifiedDate = DateTime.UtcNow,
                SourceUserWalletId = userWallet.Id,
                RelatedId = req.SerieId,
                Status = TransactionStatus.PENDING,
                Type = TransactionType.BUY_SERIES
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
                string encryptKey = _configuration[SystemSettings.CONST_PAYMENT_ENCRYPTION_KEY];
                var affiliateData = req.AffiliateCode.ToAffiliateData(encryptKey);
                if (affiliateData != null)
                {
                    purchaseHistory.AffiliateUserId = affiliateData.AffiliateUserId;
                }
            }

            await _dbContext.UserPurchaseTransactions.AddAsync(purchaseHistory);
            await _dbContext.WalletTransactions.AddAsync(transaction);
            await SyncBuySerieAsync(userId ?? Guid.Empty, transaction.Id);
            await _dbContext.SaveChangesAsync();

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
    }
}
