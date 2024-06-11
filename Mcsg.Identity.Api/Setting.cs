namespace Mcsg.Identity.Api;

using Common.SeedWork;
using Interfaces;

/// <summary>
/// Setting
/// </summary>
public class Setting : SettingBase, ISetting
{
    #region -- Implements --

    /// <summary>
    /// Notification exchange
    /// </summary>
    public string NotificationExchange { get; set; }

    /// <summary>
    /// Notification queue email
    /// </summary>
    public string NotificationQueueEmail { get; set; }

    /// <summary>
    /// Notification queue sms
    /// </summary>
    public string NotificationQueueSms { get; set; }

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
        NotificationExchange = string.Empty;
        NotificationQueueEmail = string.Empty;
        NotificationQueueSms = string.Empty;
        NotificationRoutingKey = string.Empty;
    }

    #endregion
}
