using Mcsg.Lib.Common.Constants;
using Mcsg.Lib.Common.Exceptions;
using Mcsg.Lib.Common.Models;
using Mcsg.Lib.Common.Web.Security;
using Mcsg.Lib.Data.Wallet;
using Mcsg.Lib.Data.Wallet.Enums;
using Mcsg.Wallet.Api.Constants;
using Mcsg.Wallet.Api.Extensions;
using Mcsg.Wallet.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Mcsg.Wallet.Api.Services
{
    public interface IUserPurchaseService
    {
        Task<UserPurchaseOverallResp> GetUserPurchaseTransactionsAsync(PaginatedRequest request);
        Task<PremiumPackagePurchaseResponse> GetUserPremiumPackageAsync();
    }

    public class UserPurchaseService : IUserPurchaseService
    {
        private readonly WalletDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;
        private readonly IConfiguration _configuration;
        private readonly IBankService _bankService;

        private readonly IOtpService _otpService;
        private readonly ISystemService _systemService;
        public UserPurchaseService(WalletDbContext walletDbContext,
            ICurrentUserService currentUserService,
            IOtpService otpService,
            IBankService bankService,
            ISystemService systemService,

            IConfiguration configuration)
        {
            _otpService = otpService;
            _configuration = configuration;
            _bankService = bankService;
            _currentUserService = currentUserService;
            _systemService = systemService;

            _dbContext = walletDbContext;
        }

        public async Task<UserPurchaseOverallResp> GetUserPurchaseTransactionsAsync(PaginatedRequest request)
        {
            try
            {
                UserPurchaseOverallResp results = new UserPurchaseOverallResp();

                var data = new UserPurchaseTransactionItemDetailResp();
                var query = _dbContext.WalletTransactions
                    .Include(x => x.SourceUserWallet)
                    .Include(x => x.DestinationUserWallet)
                    .Include(x => x.UserPurchaseTransactions)
                    .Where(x =>
                        (
                            (x.SourceUserWallet != null && x.SourceUserWallet.UserId == _currentUserService.Session.UserId)
                            && (x.Type == TransactionType.BUY_PREMIUM || x.Type == TransactionType.BUY_CHAPTER)
                        )
                        )
                    .OrderByDescending(x => x.CreatedDate)
                    .Select(x => new UserPurchaseTransactionItemDetailResp
                    {
                        Amount = x.Amount,
                        AmountSign = (x.Type == TransactionType.DEPOSIT
                                    || x.Type == TransactionType.REWARD
                                    || (x.Type == TransactionType.DONATE && x.DestinationUserWallet != null && x.DestinationUserWallet.UserId == _currentUserService.Session.UserId)
                                    || (x.Type == TransactionType.TRANSFER && x.DestinationUserWallet != null && x.DestinationUserWallet != null && x.DestinationUserWallet.UserId == _currentUserService.Session.UserId)
                                    ) ? "+" : "-",
                        Content = x.Content,
                        CreatedDate = x.CreatedDate,
                        FromAddress = x.IsFromSystem ? ApiMessages.FROM_SYSTEM : x.SourceUserWallet.Address,
                        ToAddress = x.DestinationUserWallet != null ? x.DestinationUserWallet.Address : string.Empty,
                        FromUser = x.IsFromSystem ? ApiMessages.FROM_SYSTEM : x.SourceUserWallet.ProfileName,
                        ReferenceNumber = x.ReferenceNumber,
                        ToUser = x.DestinationUserWallet != null ? x.DestinationUserWallet.ProfileName : string.Empty,
                        TransactionStatus = x.Status,
                        TransactionType = x.Type,
                        Id = x.Id,
                        SystemMessage = x.SystemMessage,
                        Paymethod = x.UserPurchaseTransactions.FirstOrDefault().Paymethod,
                        Thumbnail = x.UserPurchaseTransactions.FirstOrDefault().Thumbnail,
                        Title = x.UserPurchaseTransactions.FirstOrDefault().Title
                    }).AsNoTracking();


                var countQuery = _dbContext.WalletTransactions
                    .Where(x => (x.SourceUserWallet != null && x.SourceUserWallet.UserId == _currentUserService.Session.UserId)
                            && (x.Type == TransactionType.BUY_PREMIUM || x.Type == TransactionType.BUY_CHAPTER)).AsNoTracking()
                    .Select(x => new UserPurchaseTransactionItemDetailResp { Id = x.Id });
                //TODO
                var items = await query.ToListAsync();
                var totalItems = await countQuery.CountAsync();
                var currencyRatio = _bankService.GetCurrencyTypeRatios().FirstOrDefault(x => x.Type == CurrencyType.VND);

                if (items != null && items.Count() > 0)
                {
                    foreach (var item in items)
                    {
                        item.AmountOfMoney = item.Amount.ToMoney(currencyRatio.Ratio, currencyRatio.Type);
                    }

                    var orders = PaginatedList<UserPurchaseTransactionItemDetailResp>.Create(items, request.PageNumber, request.PageSize);
                    var totalAmount = items.Sum(x => x.Amount);

                    results.TotalOrders = totalItems;
                    results.TotalAmount = totalAmount;
                    results.TotalAmountOfMoney = totalAmount.ToMoney(currencyRatio.Ratio, currencyRatio.Type);
                    results.Orders = orders;
                }
                else
                {
                    float totalAmount = 0;
                    results.TotalOrders = 0;
                    results.TotalAmount = 0;
                    results.TotalAmountOfMoney = totalAmount.ToMoney(currencyRatio.Ratio, currencyRatio.Type);
                    results.Orders = new PaginatedList<UserPurchaseTransactionItemDetailResp>(0);
                }
                return results;
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ErrorCodes.QuerySyntaxWrong, ex.Message);
            }
        }

        public async Task<UserPurchaseTransactionItemDetailResp> GetUserWalletTransactionByRefNumberAsync(string referenceNumber)
        {
            var data = await _dbContext.WalletTransactions
                .Include(x => x.SourceUserWallet)
                .Include(x => x.DestinationUserWallet)
                .Include(x => x.UserPaymentMethods).ThenInclude(x => x.PaymentMethod)
                .Where(x => x.ReferenceNumber == referenceNumber)
                .AsNoTracking()
                .Select(x => new UserPurchaseTransactionItemDetailResp
                {
                    Amount = x.Amount,
                    AmountSign = (x.Type == TransactionType.DEPOSIT
                                || x.Type == TransactionType.REWARD
                                || (x.Type == TransactionType.DONATE && x.DestinationUserWallet != null && x.DestinationUserWallet.UserId == _currentUserService.Session.UserId)
                                || (x.Type == TransactionType.TRANSFER && x.DestinationUserWallet != null && x.DestinationUserWallet != null && x.DestinationUserWallet.UserId == _currentUserService.Session.UserId)
                                ) ? "+" : "-",
                    Content = x.Content,
                    CreatedDate = x.CreatedDate,
                    FromAddress = x.IsFromSystem ? ApiMessages.FROM_SYSTEM : x.SourceUserWallet.Address,
                    //FromUserId = x.IsFromSystem ? Guid.Empty : x.SourceUserWallet.UserId,
                    ToAddress = x.DestinationUserWallet != null ? x.DestinationUserWallet.Address : string.Empty,
                    FromUser = x.IsFromSystem ? ApiMessages.FROM_SYSTEM : x.SourceUserWallet.ProfileName,

                    ReferenceNumber = x.ReferenceNumber,
                    ToUser = x.DestinationUserWallet != null ? x.DestinationUserWallet.ProfileName : string.Empty,
                    //ToUserId = x.DestinationUserWallet != null ? x.DestinationUserWallet.UserId : ((x.Type == TransactionType.DEPOSIT || x.Type == TransactionType.BUY_PREMIUM) ?Guid.Empty : null),
                    TransactionStatus = x.Status,
                    TransactionType = x.Type,
                    Id = x.Id,
                    SystemMessage = x.SystemMessage
                }).FirstOrDefaultAsync();


            if (data == null)
            {
                throw new BadRequestException(ApiErrorCodes.TRANSACTION_NOT_FOUND, ApiErrorMessage.TRANSACTION_NOT_FOUND);
            }

            return data;
        }

        public async Task<PremiumPackagePurchaseResponse> GetUserPremiumPackageAsync()
        {
            var data = new PremiumPackagePurchaseResponse();
            var query = _dbContext.UserPremiumPackages
                .Include(x => x.PremiumPackage)
                .Include(x => x.UserWallet)
                .Where(x =>
                        x.UserWallet.UserId == _currentUserService.Session.UserId
                        && x.PremiumPackage.IsPackage
                        && !x.IsDelete
                    )
                .OrderByDescending(x => x.EndDate)
                .Select(x => new PremiumPackagePurchaseResponse
                {
                    Id = x.Id,
                    No = x.PremiumPackageNo ?? 0,
                    Name = x.PremiumPackage.Name,
                    Description = x.PremiumPackage.Description,
                    Price = x.PremiumPackage.Price,
                    EndDate = x.EndDate
                }).AsNoTracking();

            var userPackage = await query.FirstOrDefaultAsync();
            if (userPackage != null)
            {
                userPackage.IsActive = userPackage.EndDate >= DateTime.UtcNow;
            }

            return userPackage;
        }
    }
}
