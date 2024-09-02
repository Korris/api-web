using Newtonsoft.Json;

namespace Mcsg.Identity.Api;

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

    /// <summary>
    /// The time in minutes after which the account will be deleted
    /// </summary>
    public uint AccountDeletedAfter { get; set; }

    /// <summary>
    /// The time in minutes after which the account can be created
    /// </summary>
    public uint AccountCreatedAfter { get; set; }

    /// <summary>
    /// The remaining time in minutes after which the username can be changed
    /// </summary>
    public double UserNameChangedInRemaining { get; set; }

    /// <summary>
    /// The waiting time in minutes before the username can be changed
    /// </summary>
    public double UserNameWaitingChangedAfter { get; set; }

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
        NotificationQueueSms = string.Empty;
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
