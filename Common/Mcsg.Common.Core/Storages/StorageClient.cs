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
    /// Strategy
    /// </summary>
    public IStorageStrategy Strategy => _strategy;

    #endregion

    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="options">Options</param>
    public StorageClient(IOptions<MinioDto> options)
    {
        _auth = options.Value;
        _strategy = new StorageMinio();
        _strategy.SetAuthSender(_auth);
    }

    #endregion

    #region -- Fields --

    /// <summary>
    /// Strategy
    /// </summary>
    private IStorageStrategy _strategy;

    /// <summary>
    /// Auth sender
    /// </summary>
    private readonly MinioDto _auth;

    #endregion
}
