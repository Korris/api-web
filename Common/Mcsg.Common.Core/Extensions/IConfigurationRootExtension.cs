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

using SeedWork;
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
    /// Load OTP settings
    /// </summary>
    /// <param name="config">Configuration</param>
    /// <param name="setting">Setting object to be populated</param>
    /// <param name="prefix">Configuration prefix for settings</param>
    public static void LoadSettingOtp(this IConfigurationRoot config, SettingBase setting, string prefix)
    {
        setting.Otp.ExpiryInMinutes = setting.Otp.ExpiryInMinutes == 0
            ? config.GetConfigValue<int>($"{prefix}:ExpiryInMinutes")
            : setting.Otp.ExpiryInMinutes;

        setting.Otp.OtpLength = setting.Otp.OtpLength == 0
            ? config.GetConfigValue<int>($"{prefix}:OtpLength")
            : setting.Otp.OtpLength;

        setting.Otp.OtpTokenLength = setting.Otp.OtpTokenLength == 0
            ? config.GetConfigValue<int>($"{prefix}:OtpTokenLength")
            : setting.Otp.OtpTokenLength;

        setting.Otp.OtpExpired = setting.Otp.OtpExpired == 0
            ? config.GetConfigValue<int>($"{prefix}:OtpExpired")
            : setting.Otp.OtpExpired;
    }

    /// <summary>
    /// Load ZaloPay settings
    /// </summary>
    /// <param name="config">Configuration</param>
    /// <param name="setting">Setting object to be populated</param>
    /// <param name="prefix">Configuration prefix for settings</param>
    public static void LoadSettingZaloPay(this IConfigurationRoot config, SettingBase setting, string prefix)
    {
        setting.ZaloPay.Url ??= config.GetConfigValue<string>($"{prefix}:Url");
        setting.ZaloPay.AppId ??= config.GetConfigValue<string>($"{prefix}:AppId");
        setting.ZaloPay.AppUser ??= config.GetConfigValue<string>($"{prefix}:AppUser");
        setting.ZaloPay.Key1 ??= config.GetConfigValue<string>($"{prefix}:Key1");
        setting.ZaloPay.Key2 ??= config.GetConfigValue<string>($"{prefix}:Key2");
        setting.ZaloPay.RedirectUrl ??= config.GetConfigValue<string>($"{prefix}:RedirectUrl");
        setting.ZaloPay.CallBackUrl ??= config.GetConfigValue<string>($"{prefix}:CallBackUrl");

        setting.ZaloPay.QueryScheduleMinutes = setting.ZaloPay.QueryScheduleMinutes == 0
            ? config.GetConfigValue<int>($"{prefix}:QueryScheduleMinutes")
            : setting.ZaloPay.QueryScheduleMinutes;
    }

    /// <summary>
    /// Generic helper method to fetch values from config and cast to the desired type, with null checking.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="config">The configuration root</param>
    /// <param name="key">The configuration key</param>
    /// <returns>Value of type T if found, or default(T) if key is null or not found</returns>
    public static T GetConfigValue<T>(this IConfigurationRoot config, string key)
    {
        try
        {
            var value = config[key];
            if (string.IsNullOrEmpty(value))
            {
                return default!;
            }

            return (T)Convert.ChangeType(value, typeof(T));
        }
        catch
        {
            return default!;
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
