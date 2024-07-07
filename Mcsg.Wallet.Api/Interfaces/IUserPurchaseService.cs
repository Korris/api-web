namespace Mcsg.Wallet.Api.Interfaces;

using Mcsg.Lib.Common.Models;
using Models;

public interface IUserPurchaseService
{
    Task<UserPurchaseOverallResp> GetUserPurchaseTransactionsAsync(PaginatedRequest request);
    Task<PremiumPackagePurchaseResponse> GetUserPremiumPackageAsync();
}
