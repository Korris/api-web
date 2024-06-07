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
/// TimeSpan extension for using [this TimeSpan] only
/// </summary>
public static class TimeSpanExtension
{
    #region -- Methods --

    /// <summary>
    /// Convert TimeSpan to time elapsed
    /// </summary>
    /// <param name="t">The TimeSpan object needs to be converted</param>
    /// <returns>Return the time elapsed</returns>
    public static string ToTimeElapsed(this TimeSpan? t)
    {
        if (t == null)
        {
            return string.Empty;
        }

        var time = t.Value;
        if (time.TotalMinutes < 1)
        {
            return "just now";
        }
        else if (time.TotalMinutes < 60)
        {
            return $"{(int)time.TotalMinutes}m ago";
        }
        else if (time.TotalHours < 24)
        {
            return $"{(int)time.TotalHours}h ago";
        }
        else
        {
            return time.ToString("%d") + "d ago";
        }
    }

    #endregion
}
