namespace Mcsg.Api.Services;

using Microsoft.Extensions.Diagnostics.HealthChecks;

/// <summary>
/// Readiness gate: unhealthy until StartupWarmupHostedService has finished, so a freshly rolled pod
/// does not receive traffic while JIT, image libraries, EF and the MinIO client are still cold.
/// Tagged "ready" and exposed on /health/ready only; /health (liveness) is unaffected.
/// </summary>
public class WarmupReadinessHealthCheck : IHealthCheck
{
    public const string Tag = "ready";

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(WarmupState.IsReady
            ? HealthCheckResult.Healthy("warm")
            : HealthCheckResult.Unhealthy("warming up"));
    }
}
