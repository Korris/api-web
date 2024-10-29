using Newtonsoft.Json;

namespace Mcsg.Document.Api;

using Common.SeedWork;
using Interfaces;
using static Common.SeedWork.Dtos.StorageDto;

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
    /// Notification queue sync data
    /// </summary>
    public string NotificationQueueSyncData { get; set; }

    /// <summary>
    /// Notification queue view history
    /// </summary>
    public string NotificationQueueViewHistory { get; set; }

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
        NotificationQueueSyncData = string.Empty;
        NotificationQueueViewHistory = string.Empty;
        NotificationRoutingKey = string.Empty;
    }

    /// <summary>
    /// Load storages
    /// </summary>
    public void LoadStorages()
    {
        Minio.Storages = JsonConvert.DeserializeObject<List<MinioInstanceDto>>(Storage!)!;
    }

    #endregion
}
