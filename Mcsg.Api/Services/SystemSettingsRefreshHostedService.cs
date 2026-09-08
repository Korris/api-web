namespace Mcsg.Api.Services;

using Common.Domain;
using Interfaces;

/// <summary>
/// Re-reads system.SystemSettings every RefreshInterval and applies it onto the singleton Setting.
/// Program.cs used to load the table once at boot, so any row edited afterwards (RpcChatChat, PercentFeed, ...)
/// needed a pod restart. Now the change is visible on every replica within one interval.
/// POST api/identity/config/v1/Reload does the same on demand for the pod that receives the request.
/// Interval comes from configuration key SystemSettingsRefreshSeconds (env var of the same name), default 30s.
/// </summary>
public class SystemSettingsRefreshHostedService : BackgroundService
{
    #region -- Methods --

    public SystemSettingsRefreshHostedService(IServiceScopeFactory scopeFactory, ISetting setting, IConfiguration configuration,
        ILogger<SystemSettingsRefreshHostedService> logger)
    {
        _scopeFactory = scopeFactory;
        _setting = (Setting)setting;
        _logger = logger;

        var seconds = configuration.GetValue<int?>(IntervalKey) ?? DefaultIntervalSeconds;
        _interval = TimeSpan.FromSeconds(Math.Max(seconds, MinIntervalSeconds));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("[SETTINGS REFRESH] every {Seconds}s", _interval.TotalSeconds);

        // First run is skipped: Program.cs has just applied the same rows
        using var timer = new PeriodicTimer(_interval);
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                await RefreshOnce(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                // A transient DB error must not kill the loop; the previous values stay in effect until the next tick
                _logger.LogWarning(ex, "[SETTINGS REFRESH] failed, keeping current values");
            }
        }
    }

    /// <summary>
    /// Loads the table, applies it, and logs only the keys whose value differs from the previous load
    /// </summary>
    private async Task RefreshOnce(CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<IMcsgContext>();

        var rows = await SystemSettingsLoader.LoadAsync(context, ct);
        var current = SystemSettingsLoader.Snapshot(rows);

        var changed = current
            .Where(p => !_previous.TryGetValue(p.Key, out var old) || old != p.Value)
            .Select(p => p.Key)
            .Concat(_previous.Keys.Where(k => !current.ContainsKey(k)))
            .ToList();

        if (_previous.Count > 0 && changed.Count == 0)
        {
            return;
        }

        SystemSettingsLoader.Apply(_setting, rows);

        if (_previous.Count > 0)
        {
            _logger.LogInformation("[SETTINGS REFRESH] applied {Count} changed key(s): {Keys}", changed.Count, string.Join(", ", changed));
        }
        _previous = current;
    }

    #endregion

    #region -- Fields --

    /// <summary>
    /// Configuration / environment variable name holding the interval in seconds
    /// </summary>
    public const string IntervalKey = "SystemSettingsRefreshSeconds";

    private const int DefaultIntervalSeconds = 30;

    /// <summary>
    /// Floor so a misconfigured value cannot hammer the database
    /// </summary>
    private const int MinIntervalSeconds = 5;

    private readonly TimeSpan _interval;

    private readonly IServiceScopeFactory _scopeFactory;

    private readonly Setting _setting;

    private readonly ILogger<SystemSettingsRefreshHostedService> _logger;

    /// <summary>
    /// Key→Value of the last load; empty on the first tick so that tick always applies and seeds the baseline
    /// </summary>
    private Dictionary<string, string> _previous = new();

    #endregion
}
