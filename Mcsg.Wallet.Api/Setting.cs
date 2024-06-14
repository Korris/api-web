namespace Mcsg.Wallet.Api;

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
    /// Notification queue email
    /// </summary>
    public string NotificationQueueEmail { get; set; }

    /// <summary>
    /// Notification queue payment
    /// </summary>
    public string NotificationQueuePayment { get; set; }

    /// <summary>
    /// Notification queue sms
    /// </summary>
    public string NotificationQueueSms { get; set; }

    /// <summary>
    /// Notification queue sync data
    /// </summary>
    public string NotificationQueueSyncData { get; set; }

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
        NotificationQueueEmail = string.Empty;
        NotificationQueuePayment = string.Empty;
        NotificationQueueSms = string.Empty;
        NotificationQueueSyncData = string.Empty;
        NotificationRoutingKey = string.Empty;
    }

    #endregion
}
