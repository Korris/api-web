#region Information
/*
 * Author       : Toan Nguyen Van
 * Email        : nvt87x@gmail.com
 * Phone        : +84 345 515 010
 * ------------------------------- *
 * Create       : 2024-Jan-21 08:37
 * Update       : 2024-Jan-21 08:37
 * Checklist    : 1.0
 * Status       : New
 */
#endregion

using Microsoft.Extensions.Configuration;

namespace Mcsg.Common.Core.Extensions;

using SeedWork.Dtos;
using static SeedWork.Dtos.ConnectionDto;

/// <summary>
/// IConfigurationRoot extension for using [this IConfigurationRoot] only
/// </summary>
public static class IConfigurationRootExtension
{
    #region -- Methods --

    /// <summary>
    /// Load ConnectionDbDto settings, if available
    /// </summary>
    /// <param name="config">Configuration</param>
    /// <param name="setting">Database setting</param>
    /// <param name="prefix">Setting prefix</param>
    public static void LoadSettings(this IConfigurationRoot config, DatabaseDto setting, string prefix)
    {
        if (setting == null || string.IsNullOrWhiteSpace(prefix))
        {
            return;
        }

        config.LoadConnectionSettings(setting, prefix);

        if (string.IsNullOrWhiteSpace(setting.Name))
        {
            setting.Name = config[$"{prefix}:Name"] + "";
        }
    }

    /// <summary>
    /// Load ConnectionQueueDto settings, if available
    /// </summary>
    /// <param name="config">Configuration</param>
    /// <param name="setting">Queue setting</param>
    /// <param name="prefix">Setting prefix</param>
    public static void LoadSettings(this IConfigurationRoot config, QueueDto setting, string prefix)
    {
        if (setting == null || string.IsNullOrWhiteSpace(prefix))
        {
            return;
        }

        config.LoadConnectionSettings(setting, prefix);

        if (string.IsNullOrWhiteSpace(setting.VirtualHost))
        {
            setting.VirtualHost = config[$"{prefix}:VirtualHost"] + "";
        }
    }

    /// <summary>
    /// Load ConnectionEmailDto settings, if available
    /// </summary>
    /// <param name="config">Configuration</param>
    /// <param name="setting">Notification setting</param>
    /// <param name="prefix">Setting prefix</param>
    public static void LoadSettings(this IConfigurationRoot config, NotificationDto setting, string prefix)
    {
        if (setting == null || string.IsNullOrWhiteSpace(prefix))
        {
            return;
        }

        config.LoadConnectionSettings(setting, prefix);

        if (setting.Port == 0)
        {
            setting.Port = Convert.ToInt32(config[$"{prefix}:Port"]);
        }
        if (string.IsNullOrWhiteSpace(setting.SenderEmail))
        {
            setting.SenderEmail = config[$"{prefix}:SenderEmail"] + "";
        }
        if (setting.SenderName == null)
        {
            setting.SenderName = config[$"{prefix}:SenderName"];
        }
    }

    /// <summary>
    /// Load ConnectionDto settings, if available
    /// </summary>
    /// <param name="config">Configuration</param>
    /// <param name="setting">Connection setting</param>
    /// <param name="prefix">Setting prefix</param>
    private static void LoadConnectionSettings(this IConfigurationRoot config, ConnectionDto setting, string prefix)
    {
        if (string.IsNullOrWhiteSpace(setting.Host))
        {
            setting.Host = config[$"{prefix}:Host"] + "";
        }
        if (setting.Port == 0)
        {
            setting.Port = Convert.ToUInt16(config[$"{prefix}:Port"]);
        }
        if (string.IsNullOrWhiteSpace(setting.UserName))
        {
            setting.UserName = config[$"{prefix}:UserName"] + "";
        }
        if (string.IsNullOrWhiteSpace(setting.Password))
        {
            setting.Password = config[$"{prefix}:Password"] + "";
        }
    }

    #endregion
}
