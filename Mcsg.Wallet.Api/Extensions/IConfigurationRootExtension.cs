namespace Mcsg.Wallet.Api.Extensions;

/// <summary>
/// IConfigurationRoot extension for using [this IConfigurationRoot] only
/// </summary>
public static class IConfigurationRootExtension
{
    #region -- Methods --

    /// <summary>
    /// Load NotSetting settings, if available
    /// </summary>
    /// <param name="config">Configuration</param>
    /// <param name="setting">NotSetting setting</param>
    /// <param name="prefix">Setting prefix</param>
    public static void LoadSettings(this IConfigurationRoot config, Setting setting, string prefix)
    {
        if (string.IsNullOrEmpty(setting.NotificationExchange))
        {
            setting.NotificationExchange = config[$"{prefix}:Exchange"] + "";
        }
        if (string.IsNullOrEmpty(setting.NotificationQueuePayment))
        {
            setting.NotificationQueuePayment = config[$"{prefix}:QueuePayment"] + "";
        }
        if (string.IsNullOrEmpty(setting.NotificationQueueSms))
        {
            setting.NotificationQueueSms = config[$"{prefix}:QueueSms"] + "";
        }
    }

    #endregion
}
