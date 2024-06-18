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
/// Interface storage client
/// </summary>
public interface IStorageClient
{
    #region -- Methods --

    /// <summary>
    /// Set strategy
    /// </summary>
    /// <param name="strategy">Strategy</param>
    void SetStrategy(IStorageStrategy strategy);

    #endregion

    #region -- Properties --

    /// <summary>
    /// Strategy
    /// </summary>
    IStorageStrategy Strategy { get; }

    #endregion
}
