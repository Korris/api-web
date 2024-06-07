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

namespace Mcsg.Common.SeedWork.Interfaces;

/// <summary>
/// Marker interface to represent a business rule
/// </summary>
public interface IBusinessRule
{
    #region -- Methods --

    /// <summary>
    /// Is broken
    /// </summary>
    /// <returns>Return the result</returns>
    bool IsBroken();

    #endregion

    #region -- Properties --

    /// <summary>
    /// The message that describes the error
    /// </summary>
    string Message { get; }

    /// <summary>
    /// Next developer name
    /// </summary>
    string NextDevName { get; }

    #endregion
}
