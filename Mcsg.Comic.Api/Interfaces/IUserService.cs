namespace Mcsg.Comic.Api.Interfaces;

using Common.Core.Enums;

public interface IUserService
{
    Task SyncWalletUserReward(Guid userId, float point, RewardType type);
}
