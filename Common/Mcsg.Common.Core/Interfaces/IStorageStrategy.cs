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
    Task<Stream?> GetObject(string objectName, string? bucketName);

    /// <summary>
    /// Download object
    /// </summary>
    /// <param name="objectName">Object name (include full path and file extension)</param>
    /// <param name="fileName">File name (include full path and file extension)</param>
    /// <param name="bucketName">Bucket name (if it is null, get the default from the setting)</param>
    /// <returns>Return the result</returns>
    Task DownloadObject(string objectName, string fileName, string? bucketName);

    /// <summary>
    /// Put object
    /// </summary>
    /// <param name="fs">Stream</param>
    /// <param name="objectName">Object name (include full path and file extension)</param>
    /// <param name="bucketName">Bucket name (if it is null, get the default from the setting)</param>
    /// <returns>Return the result</returns>
    Task PutObject(Stream fs, string objectName, string? bucketName);

    /// <summary>
    /// Put object
    /// </summary>
    /// <param name="url">file URL</param>
    /// <param name="objectName">Object name (include full path and file extension)</param>
    /// <param name="bucketName">Bucket name (if it is null, get the default from the setting)</param>
    /// <returns>Return the result</returns>
    Task PutObject(string url, string objectName, string? bucketName);

    /// <summary>
    /// Presigned get object
    /// </summary>
    /// <param name="objectName">Object name</param>
    /// <param name="expiry">Expiry in seconds</param>
    /// <param name="bucketName">Bucket name (if it is null, get the default from the setting)</param>
    /// <returns></returns>
    Task<string> PresignedGetObject(string objectName, int expiry, string? bucketName);

    /// <summary>
    /// Copy object
    /// </summary>
    /// <param name="srcObjectName">Source object name</param>
    /// <param name="dstObjectName">Destination object name</param>
    /// <param name="srcBucketName">Source bucket name (if it is null, get the default from the setting)</param>
    /// <param name="dstBucketName">Destination bucket name (if it is null, get the default from the setting)</param>
    /// <returns>Return the result</returns>
    Task CopyObject(string srcObjectName, string dstObjectName, string? srcBucketName, string? dstBucketName);

    /// <summary>
    /// Tests the object's existence and returns metadata about existing objects
    /// </summary>
    /// <param name="objectName">Object name (include full path and file extension)</param>
    /// <param name="bucketName">Bucket name (if it is null, get the default from the setting)</param>
    /// <returns>Return the result</returns>
    Task<ObjectStat?> StatObjectAsync(string objectName, string? bucketName);

    /// <summary>
    /// Removes an object with given name in specific bucket
    /// </summary>
    /// <param name="objectName">Object name (include full path and file extension)</param>
    /// <param name="bucketName">Bucket name (if it is null, get the default from the setting)</param>
    /// <returns>Return the result</returns>
    Task<bool> RemoveObject(string objectName, string? bucketName);

    /// <summary>
    /// Get public URL
    /// </summary>
    /// <param name="objectName">Object name (include full path and file extension)</param>
    /// <param name="bucketNamePublic">Bucket name public (if it is null, get the default from the setting)</param>
    /// <returns>Return the public URL</returns>
    Task<string> GetPublicUrl(string objectName, string? bucketNamePublic);

    /// <summary>
    /// Move folder
    /// </summary>
    /// <param name="srcFolder">Source folder</param>
    /// <param name="dstFolder">Destination folder</param>
    /// <param name="bucketName">Bucket name (if it is null, get the default from the setting)</param>
    /// <returns>Return the result</returns>
    Task<int> MoveFolder(string srcFolder, string dstFolder, string? bucketName);

    #endregion

    #region -- Properties --

    /// <summary>
    /// Bucket name public
    /// </summary>
    string? BucketNamePublic { get; }

    #endregion
}
