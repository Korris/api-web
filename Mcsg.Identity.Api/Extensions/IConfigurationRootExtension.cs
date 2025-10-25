namespace Mcsg.Identity.Api.Extensions;

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
        if (string.IsNullOrEmpty(setting.NotificationQueueEmail))
        {
            setting.NotificationQueueEmail = config[$"{prefix}:QueueEmail"] + "";
        }
        if (string.IsNullOrEmpty(setting.NotificationQueueSms))
        {
            setting.NotificationQueueSms = config[$"{prefix}:QueueSms"] + "";
        }
        if (string.IsNullOrEmpty(setting.Origins))
        {
            setting.Origins = config[$"Origins"] + "";
        }
        if (string.IsNullOrEmpty(setting.Environment))
        {
            setting.Environment = config[$"Environment"] + "";
        }
        if (string.IsNullOrEmpty(setting.Protocols))
        {
            setting.Protocols = config[$"Protocols"] + "";
        }

    }

    #endregion
}
