namespace Mcsg.Wallet.Api.Services;

using Common.Core.Extensions;
using Domain.Interfaces;
using Interfaces;
using Requests.Notification;

public class NotificationService : BaseSettingS, INotificationService
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="context"></param>
    /// <param name="setting"></param>
    public NotificationService(IWalletContext context, ISetting setting) : base(context, setting) { }

    public async Task AddTransactionNotificationAsync(TransactionNotificationReq req)
    {
        var baseUrl = _setting.Api.Web.Realtime;
        var urlBuilder = new System.Text.StringBuilder();
        urlBuilder.Append(baseUrl != null ? baseUrl.TrimEnd('/') : "").Append("/notification/transaction");

        var url = urlBuilder.ToString();
        await url.MakePostRequest(req);
    }

    #endregion
}
