namespace Mcsg.Social.Api;

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
    /// Notification queue post react
    /// </summary>
    public string NotificationQueuePostReact { get; set; }

    /// <summary>
    /// Notification queue smart lookup
    /// </summary>
    public string NotificationQueueSmartLookup { get; set; }

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
        NotificationQueuePostReact = string.Empty;
        NotificationQueueSmartLookup = string.Empty;
        NotificationRoutingKey = string.Empty;
    }

    #endregion
}
