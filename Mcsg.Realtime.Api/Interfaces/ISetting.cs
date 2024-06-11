namespace Mcsg.Realtime.Api.Interfaces;

using Common.SeedWork.Interfaces;

/// <summary>
/// Interface setting
/// </summary>
public interface ISetting : ISettingBase
{
    #region -- Properties --

    /// <summary>
    /// Notification exchange
    /// </summary>
    string NotificationExchange { get; set; }

    /// <summary>
    /// Notification queue post comment
    /// </summary>
    string NotificationQueuePostComment { get; set; }

    /// <summary>
    /// Notification routing key
    /// </summary>
    string NotificationRoutingKey { get; set; }

    /// <summary>
    /// Allow sending email
    /// </summary>
    bool AllowSendingEmail { get; set; }

    #endregion
}
