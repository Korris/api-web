namespace Mcsg.Comic.Api.Services;

using Common.Core.Distributor;
using Common.Core.Enums;
using Interfaces;
using Lib.Common.Models;
using Models;

public partial class UserService : IUserService
{
    public UserService(DistributeManager distributeManager)
    {
        _distributeManager = distributeManager;
    }

    public async Task SyncWalletUserReward(Guid userId, float point, RewardType type)
    {
        //sync wallet profilename
        await _distributeManager.Deliver(new SyncDataDistributeItem
        {
            Data = new SyncData
            {
                TargetDb = SyncTargetDb.WALLETDB,
                TargetEntity = SyncTargetEntity.WALLET_USER_REWARD,
                Data = new Dictionary<object, object>
                {
                    { userId, new RewardSyncData {
                        Point = point,
                        Type = type
                    } }
                }
            }
        });
    }


    #region -- Fields --

    private readonly DistributeManager _distributeManager;

    #endregion
}
