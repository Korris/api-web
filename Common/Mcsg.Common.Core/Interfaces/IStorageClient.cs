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

using SeedWork.Enums;

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

    /// <summary>
    /// Retrieves an <see cref="IStorageStrategy"/> based on the specified instance type.
    /// </summary>
    /// <param name="instance">The type of the <see cref="MinioInstanceType"/> to retrieve the strategy for.</param>
    /// <returns>The corresponding <see cref="IStorageStrategy"/> object.</returns>
    IStorageStrategy GetStrategy(MinioInstanceType? instance = null);

    /// <summary>
    /// Get public URL
    /// </summary>
    /// <param name="objectName">Object name (include full path and file extension)</param>
    /// <param name="bucketNamePublic">Bucket name public (if it is null, get the default from the setting)</param>
    /// <param name="instance">The type of the <see cref="MinioInstanceType"/> to retrieve the strategy for.</param>
    /// <returns>Return the public URL</returns>
    Task<string> GetPublicUrl(string objectName, string? bucketNamePublic, MinioInstanceType? instance);

    #endregion
}
