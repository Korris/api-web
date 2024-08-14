namespace Mcsg.Comic.Api;

using Common.SeedWork;
using Interfaces;
using static Common.SeedWork.Dtos.ConnectionDto;

/// <summary>
/// Setting
/// </summary>
public class Setting : SettingBase, ISetting
{
    #region -- Implements --

    /// <summary>
    /// Database Wallet
    /// </summary>
    public DatabaseDto DbWallet { get; }

    /// <summary>
    /// Notification exchange
    /// </summary>
    public string NotificationExchange { get; set; }

    /// <summary>
    /// Notification queue post react
    /// </summary>
    public string NotificationQueuePostReact { get; set; }

    /// <summary>
    /// Notification queue smart lookup
    /// </summary>
    public string NotificationQueueSmartLookup { get; set; }

    /// <summary>
    /// Notification queue sync data
    /// </summary>
    public string NotificationQueueSyncData { get; set; }

    /// <summary>
    /// Notification queue view history
    /// </summary>
    public string NotificationQueueViewHistory { get; set; }

    /// <summary>
    /// Notification routing key
    /// </summary>
    public string NotificationRoutingKey { get; set; }

    /// <summary>
    /// Allow sending email
    /// </summary>
    public bool AllowSendingEmail { get; set; }

    #endregion

    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    public Setting()
    {
        DbWallet = new DatabaseDto();
        NotificationExchange = string.Empty;
        NotificationQueuePostReact = string.Empty;
        NotificationQueueSmartLookup = string.Empty;
        NotificationQueueSyncData = string.Empty;
        NotificationQueueViewHistory = string.Empty;
        NotificationRoutingKey = string.Empty;
    }

    #endregion
}
