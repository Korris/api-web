using StackExchange.Redis;

namespace Mcsg.Common.Core;

using Interfaces;
using static SeedWork.Dtos.ConnectionDto;

/// <summary>
/// RedisStore
/// </summary>
public class RedisStore : IRedisStore
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="s">Setting connection</param>
    public RedisStore(RedisDto s)
    {
        _s = s;

        var options = new ConfigurationOptions
        {
            EndPoints = { _s.ConnectionString },
            AllowAdmin = _s.AllowAdmin,
            Password = _s.Password
        };

        _lazy = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(options), LazyThreadSafetyMode.PublicationOnly);
    }

    #endregion

    #region -- Properties --

    /// <summary>
    /// Connection
    /// </summary>
    public ConnectionMultiplexer Connection => _lazy.Value;

    /// <summary>
    /// Redis cache
    /// </summary>
    public IDatabase RedisCache => Connection.GetDatabase(_s.Database);

    #endregion

    #region -- Fields --

    /// <summary>
    /// Lazy connection
    /// </summary>
    private readonly Lazy<ConnectionMultiplexer> _lazy;

    /// <summary>
    /// Setting connection
    /// </summary>
    private readonly RedisDto _s;

    #endregion
}
