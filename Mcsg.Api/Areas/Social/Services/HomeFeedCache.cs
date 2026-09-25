using System.Text.Json;

namespace Mcsg.Api.Areas.Social.Services;

using Common.Core.Interfaces;
using Mcsg.Api.Interfaces;

/// <summary>
/// Small Redis cache for the home feed. Fail-open: when Redis is not configured or errors, every call is a miss
/// and Redis is skipped for <see cref="CooldownSeconds"/> so a broken Redis never slows the home page down.
/// </summary>
public class HomeFeedCache
{
    #region -- Methods --

    public HomeFeedCache(IRedisStore redis, ISetting setting, ILogger<HomeFeedCache> logger)
    {
        _redis = redis;
        _logger = logger;
        _configured = !string.IsNullOrWhiteSpace(setting.Redis?.Host);
    }

    public async Task<string?> GetStringAsync(string key)
    {
        if (!Enabled)
        {
            return null;
        }
        try
        {
            var value = await _redis.RedisCache.StringGetAsync(Prefix + key);
            return value.HasValue ? value.ToString() : null;
        }
        catch (Exception ex)
        {
            Disable(ex);
            return null;
        }
    }

    public async Task SetStringAsync(string key, string value, TimeSpan ttl)
    {
        if (!Enabled)
        {
            return;
        }
        try
        {
            await _redis.RedisCache.StringSetAsync(Prefix + key, value, ttl);
        }
        catch (Exception ex)
        {
            Disable(ex);
        }
    }

    public async Task<T?> GetAsync<T>(string key) where T : class
    {
        var json = await GetStringAsync(key);
        if (json == null)
        {
            return null;
        }
        try
        {
            return JsonSerializer.Deserialize<T>(json, JsonOptions);
        }
        catch (JsonException)
        {
            return null;
        }
    }

    public Task SetAsync<T>(string key, T value, TimeSpan ttl)
    {
        return SetStringAsync(key, JsonSerializer.Serialize(value, JsonOptions), ttl);
    }

    #endregion

    #region -- Helpers --

    private bool Enabled => _configured && DateTime.UtcNow >= _disabledUntil;

    private void Disable(Exception ex)
    {
        _disabledUntil = DateTime.UtcNow.AddSeconds(CooldownSeconds);
        _logger.LogWarning(ex, "[HOME-FEED] Redis unavailable, cache skipped for {Seconds}s", CooldownSeconds);
    }

    #endregion

    #region -- Fields --

    private const string Prefix = "homefeed:";
    private const int CooldownSeconds = 60;
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    /// <summary>
    /// Shared by every request (the class is scoped, the breaker must not be)
    /// </summary>
    private static DateTime _disabledUntil = DateTime.MinValue;

    private readonly IRedisStore _redis;
    private readonly ILogger<HomeFeedCache> _logger;
    private readonly bool _configured;

    #endregion
}
