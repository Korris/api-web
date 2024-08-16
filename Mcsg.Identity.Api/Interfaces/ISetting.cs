namespace Mcsg.Identity.Api.Interfaces;

using Common.SeedWork.Interfaces;
using static Common.SeedWork.Dtos.ConnectionDto;

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
    /// Notification queue sms
    /// </summary>
    string NotificationQueueSms { get; set; }

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

    /// <summary>
    /// The remaining time in minutes after which the username can be changed
    /// </summary>
    uint UserNameChangedInRemaining { get; set; }

    /// <summary>
    /// The waiting time in minutes before the username can be changed
    /// </summary>
    uint UserNameWaitingChangedAfter { get; set; }

    #endregion
}
