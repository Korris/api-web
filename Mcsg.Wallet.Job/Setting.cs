using Newtonsoft.Json;

namespace Mcsg.Wallet.Job;

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
    /// Notification queue email
    /// </summary>
    public string NotificationQueueEmail { get; set; }

    /// <summary>
    /// Notification queue payment
    /// </summary>
    public string NotificationQueuePayment { get; set; }

    /// <summary>
    /// Notification queue sync data
    /// </summary>
    public string NotificationQueueSyncData { get; set; }

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
        NotificationQueueSyncData = string.Empty;
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
