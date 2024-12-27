namespace Mcsg.Social.Api.Interfaces;

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
    /// Notification queue post react
    /// </summary>
    string NotificationQueuePostReact { get; set; }

    /// <summary>
    /// Notification queue smart lookup
    /// </summary>
    string NotificationQueueSmartLookup { get; set; }

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
    /// Percentage of feed taken from database
    /// </summary>
    double PercentFeed { get; set; }

    /// <summary>
    /// Percentage of story taken from database
    /// </summary>
    double PercentStory { get; set; }

    /// <summary>
    /// Percentage of comic taken from database
    /// </summary>
    double PercentComic { get; set; }

    /// <summary>
    /// Number of posts taken from database
    /// </summary>
    int NumberOfPosts { get; set; }

    #endregion
}
