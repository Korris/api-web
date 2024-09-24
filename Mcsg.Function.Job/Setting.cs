using Newtonsoft.Json;

namespace Mcsg.Function.Job;

using Common.SeedWork;
using Interfaces;
using static Common.SeedWork.Dtos.ConnectionDto;
using static Common.SeedWork.Dtos.StorageDto;

/// <summary>
/// Setting
/// </summary>
public class Setting : SettingBase, ISetting
{
    #region -- Implements --

    /// <summary>
    /// Database Wallet
    /// </summary>
    public DatabaseDto DbWallet { get; }

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
    /// Notification queue post react
    /// </summary>
    public string NotificationQueuePostReact { get; set; }

    /// <summary>
    /// Notification queue smart lookup
    /// </summary>
    public string NotificationQueueSmartLookup { get; set; }

    /// <summary>
    /// Notification queue sms
    /// </summary>
    public string NotificationQueueSms { get; set; }

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

    /// <summary>
    /// The time in minutes after which the account will be deleted
    /// </summary>
    public uint AccountDeletedAfter { get; set; }

    /// <summary>
    /// The time in minutes after which the account can be created
    /// </summary>
    public uint AccountCreatedAfter { get; set; }

    #endregion

    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    public Setting()
    {
        DbWallet = new DatabaseDto();
        NotificationExchange = string.Empty;
        NotificationQueueEmail = string.Empty;
        NotificationQueuePayment = string.Empty;
        NotificationQueuePostComment = string.Empty;
        NotificationQueuePostReact = string.Empty;
        NotificationQueueSmartLookup = string.Empty;
        NotificationQueueSms = string.Empty;
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
