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

namespace Mcsg.Common.Core.Interfaces;

/// <summary>
/// Marker interface to represent a uniqueness checker
/// </summary>
public interface IUniquenessChecker
{
    #region -- Methods --

    /// <summary>
    /// Is unique
    /// </summary>
    /// <param name="data">Input data</param>
    /// <returns>Return the result</returns>
    bool IsUnique(string data);

    #endregion

    #region -- Properties --

    /// <summary>
    /// Next developer name
    /// </summary>
    string NextDevName { get; }

    #endregion
}
