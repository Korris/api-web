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

using StackExchange.Redis;

namespace Mcsg.Common.Core.Interfaces;

/// <summary>
/// Interface RedisStore
/// </summary>
public interface IRedisStore
{
    #region -- Properties --

    /// <summary>
    /// Redis cache
    /// </summary>
    public IDatabase RedisCache { get; }

    #endregion
}
