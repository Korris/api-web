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

using Minio.DataModel;

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

    /// <summary>
    /// Copy a source object into a new destination object
    /// </summary>
    /// <param name="srcObjectName">Source object name</param>
    /// <param name="dstObjectName">Destination object name</param>
    /// <param name="srcBucketName">Source bucket name (if it is null, get the default from the setting)</param>
    /// <param name="dstBucketName">Destination bucket name (if it is null, get the default from the setting)</param>
    /// <returns>Return the result</returns>
    public virtual Task CopyObject(string srcObjectName, string dstObjectName, string? srcBucketName, string? dstBucketName)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Tests the object's existence and returns metadata about existing objects
    /// </summary>
    /// <param name="objectName">Object name (include full path and file extension)</param>
    /// <param name="bucketName">Bucket name (if it is null, get the default from the setting)</param>
    /// <returns>Return the result</returns>
    public virtual Task<ObjectStat?> StatObjectAsync(string objectName, string? bucketName)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Removes an object with given name in specific bucket
    /// </summary>
    /// <param name="objectName">Object name (include full path and file extension)</param>
    /// <param name="bucketName">Bucket name (if it is null, get the default from the setting)</param>
    /// <returns>Return the result</returns>
    public virtual Task<bool> RemoveObject(string objectName, string? bucketName)
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
