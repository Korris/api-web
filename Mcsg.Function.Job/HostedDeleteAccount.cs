namespace Mcsg.Function.Job;

using Common.Core.Extensions;
using Interfaces;

/// <summary>
/// Hosted service https://www.c-sharpcorner.com/article/consuming-rabbitmq-messages-in-asp-net-core
/// </summary>
public class HostedDeleteAccount : BackgroundService
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
            var st = scope.ServiceProvider.GetRequiredService<ISetting>();
            var service = scope.ServiceProvider.GetRequiredService<IDeleteAccountService>();

            while (!stoppingToken.IsCancellationRequested)
            {
                $"Background Service is doing work.".LogInfor();

                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
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
    public HostedDeleteAccount(IServiceScopeFactory ss)
    {
        $"Initialize {nameof(HostedDeleteAccount)}".LogInfor();

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
