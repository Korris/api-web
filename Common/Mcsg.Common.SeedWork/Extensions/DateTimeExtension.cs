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

namespace Mcsg.Common.SeedWork.Extensions;

/// <summary>
/// DateTime extension for using [this DateTime] only
/// </summary>
public static class DateTimeExtension
{
    #region -- Methods --

    /// <summary>
    /// Convert DateTime to Unix time
    /// </summary>
    /// <param name="d">The DateTime object needs to be converted</param>
    /// <returns>Return the Unix time</returns>
    public static long ToUnixTime(this DateTime d)
    {
        var unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var timeSpan = d.ToUniversalTime() - unixEpoch;
        return (long)timeSpan.TotalSeconds;
    }

    /// <summary>
    /// Start of day
    /// </summary>
    /// <param name="d">Date and time</param>
    /// <param name="timezoneOffset">Timezone offset (minute)</param>
    /// <returns>Return first day at 00:00:00</returns>
    public static DateTime StartOfDay(this DateTime d, int timezoneOffset = 0)
    {
        return d.Date.AddMinutes(timezoneOffset);
    }

    /// <summary>
    /// End of day
    /// </summary>
    /// <param name="d">Date and time</param>
    /// <param name="timezoneOffset">Timezone offset (minute)</param>
    /// <returns>Return end day at 23:59:59</returns>
    public static DateTime EndOfDay(this DateTime d, int timezoneOffset = 0)
    {
        return d.Date.AddDays(1).AddSeconds(-1).AddMinutes(timezoneOffset);
    }

    #endregion
}
