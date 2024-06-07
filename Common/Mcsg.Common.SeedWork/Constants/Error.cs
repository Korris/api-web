#region Information
/*
 * Author       : Toan Nguyen Van
 * Email        : nvt87x@gmail.com
 * Phone        : +84 345 515 010
 * ------------------------------- *
 * Create       : 2024-Jan-21 07:03
 * Update       : 2024-Jan-21 07:03
 * Checklist    : 1.0
 * Status       : New
 */
#endregion

namespace Mcsg.Common.SeedWork.Constants;

/// <summary>
/// Error code
/// </summary>
public static class Error
{
    #region -- E0xx --

    /// <summary>
    /// Not found
    /// </summary>
    public const string E002 = "E002";

    #endregion

    #region -- E1xx --

    /// <summary>
    /// Password not set
    /// </summary>
    public const string E100 = "E100";

    /// <summary>
    /// Token is expired
    /// </summary>
    public const string E109 = "E109";

    #endregion
}
