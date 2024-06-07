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

namespace Mcsg.Common.SeedWork.Enums;

/// <summary>
/// User type
/// </summary>
public enum UserType
{
    /// <summary>
    /// For pet owners
    /// </summary>
    Individual = 1,

    /// <summary>
    /// For clinic, spa, ...
    /// </summary>
    Enterprise,

    /// <summary>
    /// Administrator
    /// </summary>
    Administrator
}
