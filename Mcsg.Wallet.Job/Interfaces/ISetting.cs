namespace Mcsg.Wallet.Job.Interfaces;

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
    /// Notification queue email
    /// </summary>
    string NotificationQueueEmail { get; set; }

    /// <summary>
    /// Notification queue payment
    /// </summary>
    string NotificationQueuePayment { get; set; }

    /// <summary>
    /// Notification queue sync data
    /// </summary>
    string NotificationQueueSyncData { get; set; }

    #endregion
}
