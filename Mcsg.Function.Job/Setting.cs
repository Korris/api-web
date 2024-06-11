namespace Mcsg.Function.Job;

using Common.SeedWork;
using Common.SeedWork.Dtos;
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
    /// Notification queue payment
    /// </summary>
    public string NotificationQueuePayment { get; set; }

    /// <summary>
    /// Notification queue post comment
    /// </summary>
    public string NotificationQueuePostComment { get; set; }

    /// <summary>
    /// Notification routing key
    /// </summary>
    public string NotificationRoutingKey { get; set; }

    /// <summary>
    /// Allow sending email
    /// </summary>
    public bool AllowSendingEmail { get; set; }

    /// <summary>
    /// ZaloPay
    /// </summary>
    public ZaloPayDto ZaloPay { get; }

    #endregion

    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    public Setting()
    {
        NotificationExchange = string.Empty;
        NotificationQueueEmail = string.Empty;
        NotificationQueuePayment = string.Empty;
        NotificationQueuePostComment = string.Empty;
        NotificationRoutingKey = string.Empty;
        ZaloPay = new ZaloPayDto();
    }

    #endregion
}
