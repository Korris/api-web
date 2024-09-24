namespace Mcsg.Function.Job.Interfaces;

using Common.SeedWork.Interfaces;
using static Mcsg.Common.SeedWork.Dtos.ConnectionDto;

/// <summary>
/// Interface setting
/// </summary>
public interface ISetting : ISettingBase
{
    #region -- Properties --

    /// <summary>
    /// Database Wallet
    /// </summary>
    DatabaseDto DbWallet { get; }

    /// <summary>
    /// Notification exchange
    /// </summary>
    string NotificationExchange { get; set; }

    /// <summary>
    /// Notification queue email
    /// </summary>
    string NotificationQueueEmail { get; set; }

    /// <summary>
    /// Notification queue payment
    /// </summary>
    string NotificationQueuePayment { get; set; }

    /// <summary>
    /// Notification queue post comment
    /// </summary>
    string NotificationQueuePostComment { get; set; }

    /// <summary>
    /// Notification queue post react
    /// </summary>
    string NotificationQueuePostReact { get; set; }

    /// <summary>
    /// Notification queue smart lookup
    /// </summary>
    string NotificationQueueSmartLookup { get; set; }

    /// <summary>
    /// Notification queue sms
    /// </summary>
    string NotificationQueueSms { get; set; }

    /// <summary>
    /// Notification queue sync data
    /// </summary>
    string NotificationQueueSyncData { get; set; }

    /// <summary>
    /// Notification queue view history
    /// </summary>
    string NotificationQueueViewHistory { get; set; }

    /// <summary>
    /// Notification routing key
    /// </summary>
    string NotificationRoutingKey { get; set; }

    /// <summary>
    /// Allow sending email
    /// </summary>
    bool AllowSendingEmail { get; set; }

    /// <summary>
    /// The time in minutes after which the account will be deleted
    /// </summary>
    uint AccountDeletedAfter { get; set; }

    /// <summary>
    /// The time in minutes after which the account can be created
    /// </summary>
    uint AccountCreatedAfter { get; set; }

    #endregion
}
