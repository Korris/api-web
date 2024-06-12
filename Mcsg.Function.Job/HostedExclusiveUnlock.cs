namespace Mcsg.Function.Job;

using Common.Core.Extensions;
using Interfaces;

/// <summary>
/// Hosted service https://www.c-sharpcorner.com/article/consuming-rabbitmq-messages-in-asp-net-core
/// </summary>
public class HostedExclusiveUnlock : BackgroundService
{
    #region -- Overrides --

    /// <summary>
    /// Execute async
    /// </summary>
    /// <param name="stoppingToken">Stopping token</param>
    /// <returns>A System.Threading.Tasks.Task that represents the long running operations</returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();

        using (var scope = _ss.CreateScope())
        {
            var service = scope.ServiceProvider.GetRequiredService<IExclusiveUnlockService>();

            while (!stoppingToken.IsCancellationRequested)
            {
                $"Background Service is doing work.".LogInfor();

                //TODO - The job runs every 5 hours and needs to get the configuration
                await Task.Delay(TimeSpan.FromHours(5), stoppingToken);
                await service.Run();
            }
        }
    }

    #endregion

    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="ss">Service scope factory</param>
    /// <exception cref="ArgumentNullException"></exception>
    public HostedExclusiveUnlock(IServiceScopeFactory ss)
    {
        $"Initialize {nameof(HostedExclusiveUnlock)}".LogInfor();

        _ss = ss ?? throw new ArgumentNullException(nameof(ss));
    }

    #endregion

    #region -- Fields --

    /// <summary>
    /// Service scope factory
    /// </summary>
    private readonly IServiceScopeFactory _ss;

    #endregion
}
