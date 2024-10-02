namespace Mcsg.Wallet.Job.Interfaces;

using Lib.Common.Models;

public interface ISyncDataService
{
    Task SyncWalletUserInfoAsync(SyncData data);
    Task SyncWalletUserRewardAsync(SyncData data);
    Task SyncUserPremiumAsync(SyncData data);
    Task SyncUserBuyChapterAsync(SyncData data);
    Task SyncUserBuySeriesAsync(SyncData data);
}
