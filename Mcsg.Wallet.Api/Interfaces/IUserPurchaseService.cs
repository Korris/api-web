namespace Mcsg.Wallet.Api.Interfaces;

using Models;
using Requests;

public interface IUserPurchaseService
{
    Task<UserPurchaseOverallResp> GetUserPurchaseTransactionsAsync(UserPurchasePaginatedR request);
    Task<PremiumPackagePurchaseResponse> GetUserPremiumPackageAsync();
}
