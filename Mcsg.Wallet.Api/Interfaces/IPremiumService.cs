namespace Mcsg.Wallet.Api.Interfaces;

using Models;

public interface IPremiumService
{
    Task<IEnumerable<PremiumPackageResponse>> GetPremiumPackage();
    Task<BuyItemResp> SelectPremiumPackage(int? packageNo);
    Task<bool> BuyPremium(BuyPremiumReq req);
    Task<BuyItemResp> SelectChapterPackage(Guid chapterId);
    Task<bool> BuyChapter(BuyChapterReq req);
    Task<bool> BuySerieAsync(BuySerieReq req);
}
