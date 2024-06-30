using Microsoft.EntityFrameworkCore;

namespace Mcsg.Identity.Api.Services
{
    using Constants;
    using Helpers;
    using Interfaces;
    using Lib.Common.Web.Security;
    using Lib.Data.Domain.Entities;
    using Lib.Data.Wallet;
    using Lib.Data.Wallet.Entities;

    public class UserWalletService : IUserWalletService
    {
        private readonly WalletDbContext _walletDbContext;
        private readonly ICurrentUserService _currentUserService;
        public UserWalletService(WalletDbContext walletDbContext,
            ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
            _walletDbContext = walletDbContext;
        }

        public async Task InitUserWalletAsync(User user)
        {
            var wallet = await _walletDbContext.UserWallets.AddAsync(new UserWallet
            {
                Address = await GennerateWalletAddress(),
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow,
                Id = Guid.NewGuid(),
                Point = 0,
                RewardPoint = SystemConfig.DefaultRewardPoint,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                ProfileName = user.ProfileName,
                Status = Lib.Data.Wallet.Enums.UserWalletStatus.APPROVED,
                UserId = user.Id,
                WalletSettingId = (await _walletDbContext.WalletSettings.FirstOrDefaultAsync()).Id
            });

            await _walletDbContext.WalletTransactions.AddAsync(new WalletTransaction
            {
                CreatedDate = DateTime.UtcNow,
                Id = Guid.NewGuid(),
                Amount = SystemConfig.DefaultRewardPoint,
                Content = ApiMessages.REWARD_FOR_NEW_USER,
                IsFromSystem = true,
                DestinationUserWalletId = wallet.Entity.Id,
                ReferenceNumber = await GennerateWalletTransactionNumber(),
                ModifiedDate = DateTime.UtcNow,
                Status = Lib.Data.Wallet.Enums.TransactionStatus.SUCCESS,
                Type = Lib.Data.Wallet.Enums.TransactionType.REWARD,
                IsConfirmed = true,
            });
            await _walletDbContext.SaveChangesAsync();
        }

        private async Task<string> GennerateWalletAddress()
        {
            string address = StringHelper.GetRandomString(SystemConfig.WalletAddressLength).ToLower();
            while (true)
            {
                var isExisted = await _walletDbContext.UserWallets.AnyAsync(x => x.Address == address);
                if (isExisted)
                {
                    address = StringHelper.GetRandomString(SystemConfig.WalletAddressLength).ToLower();
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
            string number = StringHelper.GetRandomString(SystemConfig.WalletTransactionLength).ToLower();
            while (true)
            {
                var isExisted = await _walletDbContext.WalletTransactions.AnyAsync(x => x.ReferenceNumber == number);
                if (isExisted)
                {
                    number = StringHelper.GetRandomString(SystemConfig.WalletTransactionLength).ToLower();
                }
                else
                {
                    break;
                }
            }
            return number;
        }
    }
}
