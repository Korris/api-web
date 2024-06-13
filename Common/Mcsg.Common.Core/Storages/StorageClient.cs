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

using Microsoft.Extensions.Options;

namespace Mcsg.Common.Core.Storages;

using Interfaces;
using static SeedWork.Dtos.StorageDto;

/// <summary>
/// Storage client
/// </summary>
public class StorageClient : IStorageClient
{
    #region -- Implements --

    /// <summary>
    /// Set strategy
    /// </summary>
    /// <param name="strategy">Strategy</param>
    public void SetStrategy(IStorageStrategy strategy)
    {
        _strategy = strategy;
        _strategy.SetAuthSender(_auth);
    }

    /// <summary>
    /// Get object
    /// </summary>
    /// <param name="objectName">Object name (include full path and file extension)</param>
    /// <param name="bucketName">Bucket name (if it is null, get the default from the setting)</param>
    /// <returns>Return the result</returns>
    public Task<Stream> GetObject(string objectName, string? bucketName)
    {
        ArgumentNullException.ThrowIfNull(_strategy, nameof(_strategy));

        return _strategy.GetObject(objectName, bucketName);
    }

    /// <summary>
    /// Put object
    /// </summary>
    /// <param name="fs">Stream</param>
    /// <param name="objectName">Object name (include full path and file extension)</param>
    /// <param name="bucketName">Bucket name (if it is null, get the default from the setting)</param>
    /// <returns>Return the result</returns>
    public Task PutObject(Stream fs, string objectName, string? bucketName)
    {
        ArgumentNullException.ThrowIfNull(_strategy, nameof(_strategy));

        return _strategy.PutObject(fs, objectName, bucketName);
    }

    /// <summary>
    /// Presigned get object
    /// </summary>
    /// <param name="objectName">Object name</param>
    /// <param name="expiry">Expiry in seconds</param>
    /// <param name="bucketName">Bucket name (if it is null, get the default from the setting)</param>
    /// <returns></returns>
    public Task<string> PresignedGetObject(string objectName, int expiry, string? bucketName)
    {
        ArgumentNullException.ThrowIfNull(_strategy, nameof(_strategy));

        return _strategy.PresignedGetObject(objectName, expiry, bucketName);
    }

    #endregion

    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="options">Options</param>
    public StorageClient(IOptions<MinioDto> options)
    {
        _auth = options.Value;
    }

    #endregion

    #region -- Fields --

    /// <summary>
    /// Strategy
    /// </summary>
    private IStorageStrategy? _strategy;

    /// <summary>
    /// Auth sender
    /// </summary>
    private MinioDto _auth;

    #endregion
}
