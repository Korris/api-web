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

namespace Mcsg.Common.Core.Storages;

using Interfaces;
using static SeedWork.Dtos.StorageDto;

/// <summary>
/// Storage strategy
/// </summary>
public class StorageStrategy : IStorageStrategy
{
    #region -- Implements --

    /// <summary>
    /// Set auth sender
    /// </summary>
    /// <param name="auth">Auth sender</param>
    public void SetAuthSender(MinioDto auth)
    {
        _auth = auth;
    }

    /// <summary>
    /// Get object
    /// </summary>
    /// <param name="objectName">Object name (include full path and file extension)</param>
    /// <param name="bucketName">Bucket name (if it is null, get the default from the setting)</param>
    /// <returns>Return the result</returns>
    public virtual Task<Stream> GetObject(string objectName, string? bucketName)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Put object
    /// </summary>
    /// <param name="fs">Stream</param>
    /// <param name="objectName">Object name (include full path and file extension)</param>
    /// <param name="bucketName">Bucket name (if it is null, get the default from the setting)</param>
    /// <returns>Return the result</returns>
    public virtual Task PutObject(Stream fs, string objectName, string? bucketName)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Presigned get object
    /// </summary>
    /// <param name="objectName">Object name</param>
    /// <param name="expiry">Expiry in seconds</param>
    /// <param name="bucketName">Bucket name (if it is null, get the default from the setting)</param>
    /// <returns></returns>
    public virtual Task<string> PresignedGetObject(string objectName, int expiry, string? bucketName)
    {
        throw new NotImplementedException();
    }

    #endregion

    #region -- Fields --

    /// <summary>
    /// Auth sender
    /// </summary>
    protected MinioDto? _auth { get; set; }

    #endregion
}
