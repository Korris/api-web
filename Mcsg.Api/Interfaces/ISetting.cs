namespace Mcsg.Api.Interfaces;

using Common.SeedWork.Interfaces;
using static Common.SeedWork.Dtos.ConnectionDto;

/// <summary>
/// Unified setting interface - merged from all 6 services
/// </summary>
public interface ISetting : ISettingBase
{
    #region -- Notification (shared) --

    string NotificationExchange { get; set; }
    string NotificationRoutingKey { get; set; }
    bool AllowSendingEmail { get; set; }

    #endregion

    #region -- Notification queues (Comic/Story/Document/Social) --

    string NotificationQueuePostReact { get; set; }
    string NotificationQueueSmartLookup { get; set; }
    string NotificationQueueSyncData { get; set; }
    string NotificationQueueViewHistory { get; set; }

    #endregion

    #region -- Notification queues (Identity) --

    string NotificationQueueEmail { get; set; }
    string NotificationQueueSms { get; set; }

    #endregion

    #region -- Notification queues (Realtime) --

    string NotificationQueuePostComment { get; set; }

    #endregion

    #region -- Identity --

    string ReCaptchaSecretKey { get; set; }
    string UsernameIsReserved { get; set; }
    uint AccountDeletedAfter { get; set; }
    uint AccountCreatedAfter { get; set; }
    double UserNameChangedInRemaining { get; set; }
    double UserNameWaitingChangedAfter { get; set; }

    #endregion

    #region -- Social --

    int NumberOfPosts { get; set; }
    double PercentFeed { get; set; }
    double PercentComic { get; set; }
    double PercentStory { get; set; }
    double PercentDocument { get; set; }
    double PercentTapShow { get; set; }

    #endregion

    #region -- Realtime --

    RedisDto Redis { get; }

    #endregion
}
