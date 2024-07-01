namespace Mcsg.Identity.Api.Interfaces;

using Common.Core.Dtos;
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
    /// API
    /// </summary>
    ApiDto Api { get; }

    #endregion
}
