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

namespace Mcsg.Common.Core.Extensions;

/// <summary>
/// DateTimeExtension extension for using [this DateTimeExtension] only
/// </summary>
public static class DateTimeExtension
{
    #region -- Methods --

    /// <summary>
    /// IsNewChapter
    /// </summary>
    /// <param name="dt">DateTime</param>
    /// <returns>Return the result</returns>
    public static bool IsNewChapter(this DateTime dt) => dt.AddDays(2) >= DateTime.UtcNow;

    #endregion
}
