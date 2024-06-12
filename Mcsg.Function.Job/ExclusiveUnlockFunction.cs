namespace Mcsg.Function.Job;

using Common.Core.Extensions;
using Interfaces;

/// <summary>
/// ExclusiveUnlockFunction
/// </summary>
public class ExclusiveUnlockFunction : BackgroundService
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
            var exclusiveUnlockService = scope.ServiceProvider.GetRequiredService<IExclusiveUnlockService>();

            while (!stoppingToken.IsCancellationRequested)
            {
                $"Background Service is doing work.".LogInfor();

                //TODO - The job runs every 5 hours and needs to get the configuration
                await Task.Delay(TimeSpan.FromHours(5), stoppingToken);
                await exclusiveUnlockService.Run();
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
    public ExclusiveUnlockFunction(IServiceScopeFactory ss)
    {
        $"Initialize {nameof(ExclusiveUnlockFunction)}".LogInfor();

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
