namespace Mcsg.Wallet.Job.Interfaces;

using Common.Models;

public interface ISyncDataService
{
    Task SyncWalletUserInfoAsync(SyncData data);
    Task SyncWalletUserRewardAsync(SyncData data);
    Task SyncUserPremiumAsync(SyncData data);
    Task SyncUserBuyChapterAsync(SyncData data);
    Task SyncUserBuySeriesAsync(SyncData data);
}
