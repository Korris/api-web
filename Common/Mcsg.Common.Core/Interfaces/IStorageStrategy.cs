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

using static SeedWork.Dtos.StorageDto;

/// <summary>
/// Interface notification<br/>
/// https://refactoring.guru/design-patterns/strategy/csharp/example
/// </summary>
public interface IStorageStrategy
{
    #region -- Methods --

    /// <summary>
    /// Set auth sender
    /// </summary>
    /// <param name="auth">Auth sender</param>
    void SetAuthSender(MinioDto auth);

    /// <summary>
    /// Get object
    /// </summary>
    /// <param name="objectName">Object name (include full path and file extension)</param>
    /// <param name="bucketName">Bucket name (if it is null, get the default from the setting)</param>
    /// <returns>Return the result</returns>
    Task<Stream> GetObject(string objectName, string? bucketName);

    /// <summary>
    /// Put object
    /// </summary>
    /// <param name="fs">Stream</param>
    /// <param name="objectName">Object name (include full path and file extension)</param>
    /// <param name="bucketName">Bucket name (if it is null, get the default from the setting)</param>
    /// <returns>Return the result</returns>
    Task PutObject(Stream fs, string objectName, string? bucketName);

    /// <summary>
    /// Presigned get object
    /// </summary>
    /// <param name="objectName">Object name</param>
    /// <param name="expiry">Expiry in seconds</param>
    /// <param name="bucketName">Bucket name (if it is null, get the default from the setting)</param>
    /// <returns></returns>
    Task<string> PresignedGetObject(string objectName, int expiry, string? bucketName);

    #endregion
}
