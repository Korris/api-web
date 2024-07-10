using Microsoft.EntityFrameworkCore;

namespace Mcsg.Wallet.Api.Services;

using Common.Core.Enums;
using Interfaces;
using Lib.Common.Distributor;
using Lib.Common.Models;
using Lib.Data.Wallet;
using Lib.Data.Wallet.Enums;
using Models;

public class SystemService : ISystemService
{
    private readonly WalletDbContext _dbContext;
    private readonly DistributeManager _distributeManager;
    public SystemService(WalletDbContext walletDbContext, DistributeManager distributeManager)
    {
        _dbContext = walletDbContext;
        _distributeManager = distributeManager;
    }

    public async Task<bool> SendAdminNoti(string action, string fromUser, string content)
    {
        var selectSetting = await _dbContext.WalletSettingDetails.Where(x => x.Name == nameof(WalletSettingDetailType.WithDrawNotify)).FirstOrDefaultAsync();
        var listEmailAdmin = selectSetting.Value.Split(',');
        foreach (var email in listEmailAdmin)
        {
            await CreateEmailNotiAsync(action, email, fromUser, content);
        }
        return true;
    }
    private async Task CreateEmailNotiAsync(string action, string to, string fromUser, string content)
    {
        var body = $"{action},{fromUser},{content}";
        var jobType = action == nameof(TransactionType.DEPOSIT) ? JobType.DepositNoti : JobType.WithDrawNoti;
        var emailJob = new EmailJobDistributeItem
        {
            Email = new Email { To = to, Body = body },
            JobType = jobType
        };

        await _distributeManager.Deliver(emailJob);
    }
}
