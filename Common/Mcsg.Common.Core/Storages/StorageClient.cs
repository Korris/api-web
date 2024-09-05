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
using SeedWork.Enums;
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
        _strategy.Add(strategy);
    }

    /// <summary>
    /// Retrieves an <see cref="IStorageStrategy"/> based on the specified instance type.
    /// </summary>
    /// <param name="instance">The type of the <see cref="MinioInstanceType"/> to retrieve the strategy for.</param>
    /// <returns>The corresponding <see cref="IStorageStrategy"/> object.</returns>
    public IStorageStrategy GetStrategy(MinioInstanceType? instance)
    {
        instance = instance ?? MinioInstanceType.Default;
        return _strategy[(int)instance];
    }

    /// <summary>
    /// Get public URL
    /// </summary>
    /// <param name="objectName">Object name (include full path and file extension)</param>
    /// <param name="bucketNamePublic">Bucket name public (if it is null, get the default from the setting)</param>
    /// <param name="instance">The type of the <see cref="MinioInstanceType"/> to retrieve the strategy for.</param>
    /// <returns>Return the public URL</returns>
    public Task<string> GetPublicUrl(string objectName, string? bucketNamePublic, MinioInstanceType? instance)
    {
        return GetStrategy(instance).GetPublicUrl(objectName, bucketNamePublic);
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
        _strategy = [];

        foreach (var i in _auth.Storages)
        {
            var strategy = new StorageMinio();
            strategy.SetAuthSender(i);
            _strategy.Add(strategy);
        }
    }

    #endregion

    #region -- Fields --

    /// <summary>
    /// Strategy
    /// </summary>
    private List<IStorageStrategy> _strategy;

    /// <summary>
    /// Auth sender
    /// </summary>
    private readonly MinioDto _auth;

    #endregion
}
