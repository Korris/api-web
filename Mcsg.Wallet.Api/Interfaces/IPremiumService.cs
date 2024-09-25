namespace Mcsg.Wallet.Api.Interfaces;

using Models;
using Requests;

public interface IPremiumService
{
    Task<IEnumerable<PremiumPackageResponse>> GetPremiumPackage();
    Task<BuyItemResp> SelectPremiumPackage(Guid userId, int? packageNo);
    Task<bool> BuyPremium(PremiumBuyPremiumR req);
    Task<BuyItemResp> SelectChapterPackage(Guid userId, Guid chapterId);
    Task<bool> BuyChapter(PremiumBuyChapterR req);
    Task<bool> BuySerieAsync(PremiumBuySerieR req);
}
