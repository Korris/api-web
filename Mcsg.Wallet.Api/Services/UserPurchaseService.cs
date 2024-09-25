using Microsoft.EntityFrameworkCore;

namespace Mcsg.Wallet.Api.Services;

using Common.SeedWork.Exceptions;
using Constants;
using Domain.Enums;
using Domain.Interfaces;
using Extensions;
using Interfaces;
using Lib.Common.Constants;
using Lib.Common.Models;
using Models;
using Requests;

public class UserPurchaseService : BaseS, IUserPurchaseService
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="context"></param>
    /// <param name="bankService"></param>
    public UserPurchaseService(IWalletContext context, IBankService bankService) : base(context)
    {
        _bankService = bankService;
    }

    public async Task<UserPurchaseOverallResp> GetUserPurchaseTransactionsAsync(UserPurchasePaginatedR request)
    {
        try
        {
            var results = new UserPurchaseOverallResp();
            var userId = request.UserId;

            var data = new UserPurchaseTransactionItemDetailResp();
            var query = _context.WalletTransactions
                .Include(x => x.SourceUserWallet)
                .Include(x => x.DestinationUserWallet)
                .Include(x => x.UserPurchaseTransactions)
                .Where(x =>
                    (
                        (x.SourceUserWallet != null && x.SourceUserWallet.UserId == userId)
                        && (x.Type == TransactionType.BuyPremium || x.Type == TransactionType.BuyChapter)
                    )
                    )
                .OrderByDescending(x => x.CreatedOn)
                .Select(x => new UserPurchaseTransactionItemDetailResp
                {
                    Amount = x.Amount,
                    AmountSign = (x.Type == TransactionType.Deposit
                                || x.Type == TransactionType.Reward
                                || (x.Type == TransactionType.Donate && x.DestinationUserWallet != null && x.DestinationUserWallet.UserId == userId)
                                || (x.Type == TransactionType.Transfer && x.DestinationUserWallet != null && x.DestinationUserWallet != null && x.DestinationUserWallet.UserId == userId)
                                ) ? "+" : "-",
                    Content = x.Content,
                    CreatedOn = x.CreatedOn,
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


            var countQuery = _context.WalletTransactions
                .Where(x => (x.SourceUserWallet != null && x.SourceUserWallet.UserId == userId)
                        && (x.Type == TransactionType.BuyPremium || x.Type == TransactionType.BuyChapter)).AsNoTracking()
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

    public async Task<UserPurchaseTransactionItemDetailResp> GetUserWalletTransactionByRefNumberAsync(Guid userId, string referenceNumber)
    {
        var data = await _context.WalletTransactions
            .Include(x => x.SourceUserWallet)
            .Include(x => x.DestinationUserWallet)
            .Include(x => x.UserPaymentMethods).ThenInclude(x => x.PaymentMethod)
            .Where(x => x.ReferenceNumber == referenceNumber)
            .AsNoTracking()
            .Select(x => new UserPurchaseTransactionItemDetailResp
            {
                Amount = x.Amount,
                AmountSign = (x.Type == TransactionType.Deposit
                            || x.Type == TransactionType.Reward
                            || (x.Type == TransactionType.Donate && x.DestinationUserWallet != null && x.DestinationUserWallet.UserId == userId)
                            || (x.Type == TransactionType.Transfer && x.DestinationUserWallet != null && x.DestinationUserWallet != null && x.DestinationUserWallet.UserId == userId)
                            ) ? "+" : "-",
                Content = x.Content,
                CreatedOn = x.CreatedOn,
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

    public async Task<PremiumPackagePurchaseResponse> GetUserPremiumPackageAsync(Guid userId)
    {
        var data = new PremiumPackagePurchaseResponse();
        var query = _context.UserPremiumPackages
            .Include(x => x.PremiumPackage)
            .Include(x => x.UserWallet)
            .Where(x =>
                    x.UserWallet.UserId == userId
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

    #endregion

    #region -- Fields --

    private readonly IBankService _bankService;

    #endregion
}
