using Microsoft.EntityFrameworkCore;

namespace Mcsg.Identity.Api.Services;

using Common.Domain.Entities;
using Common.SeedWork.Extensions;
using Constants;
using Interfaces;
using Lib.Data.Wallet;
using Lib.Data.Wallet.Entities;

public class UserWalletService : IUserWalletService
{
    #region -- Methods --

    public UserWalletService(WalletDbContext walletDbContext)
    {
        _walletDbContext = walletDbContext;
    }

    public async Task InitUserWalletAsync(User user, bool createWalletTransaction)
    {
        var walletSetting = await _walletDbContext.WalletSettings.FirstOrDefaultAsync();

        var wallet = await _walletDbContext.UserWallets.AddAsync(new UserWallet
        {
            Address = await GennerateWalletAddress(),
            CreatedOn = DateTime.UtcNow,
            ModifiedOn = DateTime.UtcNow,
            Id = Guid.NewGuid(),
            Point = 0,
            RewardPoint = createWalletTransaction ? DefaultRewardPoint : 0,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            ProfileName = user.ProfileName,
            Status = Lib.Data.Wallet.Enums.UserWalletStatus.APPROVED,
            UserId = user.Id,
            WalletSettingId = walletSetting!.Id
        });

        if (!createWalletTransaction)
        {
            await _walletDbContext.SaveChangesAsync();
            return;
        }

        await _walletDbContext.WalletTransactions.AddAsync(new WalletTransaction
        {
            CreatedOn = DateTime.UtcNow,
            Id = Guid.NewGuid(),
            Amount = DefaultRewardPoint,
            Content = ApiMessages.REWARD_FOR_NEW_USER,
            IsFromSystem = true,
            DestinationUserWalletId = wallet.Entity.Id,
            ReferenceNumber = await GennerateWalletTransactionNumber(),
            ModifiedOn = DateTime.UtcNow,
            Status = Lib.Data.Wallet.Enums.TransactionStatus.SUCCESS,
            Type = Lib.Data.Wallet.Enums.TransactionType.REWARD,
            IsConfirmed = true,
        });
        await _walletDbContext.SaveChangesAsync();
    }

    private async Task<string> GennerateWalletAddress()
    {
        string address = WalletAddressLength.GetRandomString().ToLower();
        while (true)
        {
            var isExisted = await _walletDbContext.UserWallets.AnyAsync(x => x.Address == address);
            if (isExisted)
            {
                address = WalletAddressLength.GetRandomString().ToLower();
            }
            else
            {
                break;
            }
        }
        return address;
    }

    private async Task<string> GennerateWalletTransactionNumber()
    {
        string number = WalletTransactionLength.GetRandomString().ToLower();
        while (true)
        {
            var isExisted = await _walletDbContext.WalletTransactions.AnyAsync(x => x.ReferenceNumber == number);
            if (isExisted)
            {
                number = WalletTransactionLength.GetRandomString().ToLower();
            }
            else
            {
                break;
            }
        }
        return number;
    }

    #endregion

    #region -- Fields --

    private readonly WalletDbContext _walletDbContext;

    #endregion

    #region -- Constants --

    private const int WalletAddressLength = 12;
    private const int WalletTransactionLength = 12;
    private const int DefaultRewardPoint = 200000;

    #endregion
}
