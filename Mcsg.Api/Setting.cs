using Newtonsoft.Json;

namespace Mcsg.Api;

using Common.SeedWork;
using Interfaces;
using static Common.SeedWork.Dtos.ConnectionDto;
using static Common.SeedWork.Dtos.StorageDto;

/// <summary>
/// Unified setting - merged from all 6 services
/// </summary>
public class Setting : SettingBase, ISetting
{
    #region -- Notification (shared) --

    public string NotificationExchange { get; set; }
    public string NotificationRoutingKey { get; set; }
    public bool AllowSendingEmail { get; set; }

    #endregion

    #region -- Notification queues (Comic/Story/Document/Social) --

    public string NotificationQueuePostReact { get; set; }
    public string NotificationQueueSmartLookup { get; set; }
    public string NotificationQueueSyncData { get; set; }
    public string NotificationQueueViewHistory { get; set; }

    #endregion

    #region -- Notification queues (Identity) --

    public string NotificationQueueEmail { get; set; }
    public string NotificationQueueSms { get; set; }

    #endregion

    #region -- Notification queues (Realtime) --

    public string NotificationQueuePostComment { get; set; }

    #endregion

    #region -- Identity --

    public string ReCaptchaSecretKey { get; set; }
    public string UsernameIsReserved { get; set; }
    public uint AccountDeletedAfter { get; set; }
    public uint AccountCreatedAfter { get; set; }
    public double UserNameChangedInRemaining { get; set; }
    public double UserNameWaitingChangedAfter { get; set; }

    #endregion

    #region -- Social --

    public int NumberOfPosts { get; set; }
    public double PercentFeed { get; set; }
    public double PercentComic { get; set; }
    public double PercentStory { get; set; }
    public double PercentDocument { get; set; }

    #endregion

    #region -- Realtime --

    public RedisDto Redis { get; }

    #endregion

    #region -- Methods --

    public Setting()
    {
        NotificationExchange = string.Empty;
        NotificationRoutingKey = string.Empty;
        NotificationQueuePostReact = string.Empty;
        NotificationQueueSmartLookup = string.Empty;
        NotificationQueueSyncData = string.Empty;
        NotificationQueueViewHistory = string.Empty;
        NotificationQueueEmail = string.Empty;
        NotificationQueueSms = string.Empty;
        NotificationQueuePostComment = string.Empty;
        ReCaptchaSecretKey = string.Empty;
        UsernameIsReserved = string.Empty;
        Redis = new RedisDto();
    }

    /// <summary>
    /// Load storages from JSON config
    /// </summary>
    public void LoadStorages()
    {
        Minio.Storages = JsonConvert.DeserializeObject<List<MinioInstanceDto>>(Storage!)!;
    }

    #endregion
}
