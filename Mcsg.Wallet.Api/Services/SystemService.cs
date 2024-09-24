using Microsoft.EntityFrameworkCore;

namespace Mcsg.Wallet.Api.Services;

using Common.Core.Distributor;
using Common.Core.Enums;
using Domain;
using Domain.Enums;
using Interfaces;
using Lib.Common.Models;
using Models;

public class SystemService : BaseS, ISystemService
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="context"></param>
    /// <param name="distributeManager"></param>
    public SystemService(WalletContext context, DistributeManager distributeManager) : base(context)
    {
        _distributeManager = distributeManager;
    }

    public async Task<bool> SendAdminNoti(string action, string fromUser, string content)
    {
        var selectSetting = await _context.WalletSettingDetails.Where(x => x.Name == nameof(WalletSettingDetailType.WithDrawNotify)).FirstOrDefaultAsync();
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
        var jobType = action == nameof(TransactionType.Deposit) ? JobType.DepositNoti : JobType.WithDrawNoti;
        var emailJob = new EmailJobDistributeItem
        {
            Email = new Email { To = to, Body = body },
            JobType = jobType
        };

        await _distributeManager.Deliver(emailJob);
    }

    #endregion

    #region -- Fields --

    private readonly DistributeManager _distributeManager;

    #endregion
}
