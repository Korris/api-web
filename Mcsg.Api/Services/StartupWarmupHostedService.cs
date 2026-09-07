namespace Mcsg.Api.Services;

using System.Diagnostics;
using Common.Core.Enums;
using Common.Core.Extensions;
using Common.Core.Interfaces;
using Common.Domain;
using Common.SeedWork.Enums;
using Common.SeedWork.Extensions;
using ImageMagick;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Pays the cold-start cost once at boot instead of on the first user request.
/// Measured on staging: first upload-media on a fresh pod took 11.6s, the second 3.1s. The difference is JIT,
/// Magick.NET/ImageSharp native init, EF model/query compilation and the MinIO SDK region lookup.
/// Readiness (/health/ready) stays unhealthy until this finishes, so k8s keeps traffic on the old pod meanwhile.
/// </summary>
public class StartupWarmupHostedService : BackgroundService
{
    #region -- Methods --

    public StartupWarmupHostedService(IServiceScopeFactory scopeFactory, IStorageClient storageClient, ILogger<StartupWarmupHostedService> logger)
    {
        _scopeFactory = scopeFactory;
        _storageClient = storageClient;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Hosted services start sequentially and Kestrel is the last one: yield first so the CPU-bound image step
        // does not delay the port binding (liveness/readiness would be connection-refused meanwhile)
        await Task.Yield();

        var total = Stopwatch.StartNew();
        try
        {
            await Step("image-pipeline", WarmImagePipeline, stoppingToken);
            await Step("database", WarmDatabase, stoppingToken);
            await Step("minio", WarmMinio, stoppingToken);
        }
        finally
        {
            // Never keep a pod out of rotation because a warm-up step failed; the step is only an optimisation
            WarmupState.MarkReady();
            _logger.LogInformation("[WARMUP] done in {Ms}ms, pod ready", total.ElapsedMilliseconds);
        }
    }

    /// <summary>
    /// Runs one step with its own deadline. A hung dependency (MinIO, DB) must not hold readiness for minutes:
    /// the step is abandoned after StepTimeout and the pod is marked ready anyway.
    /// </summary>
    private async Task Step(string name, Func<CancellationToken, Task> action, CancellationToken stoppingToken)
    {
        var sw = Stopwatch.StartNew();
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
        cts.CancelAfter(StepTimeout);
        try
        {
            var work = action(cts.Token);
            var finished = await Task.WhenAny(work, Task.Delay(StepTimeout, cts.Token));
            if (finished != work)
            {
                _logger.LogWarning("[WARMUP] {Step} timed out after {Ms}ms, continuing", name, sw.ElapsedMilliseconds);
                return;
            }
            await work;
            _logger.LogInformation("[WARMUP] {Step} ok in {Ms}ms", name, sw.ElapsedMilliseconds);
        }
        catch (Exception ex) when (ex is not OperationCanceledException || !stoppingToken.IsCancellationRequested)
        {
            _logger.LogWarning(ex, "[WARMUP] {Step} failed after {Ms}ms, continuing", name, sw.ElapsedMilliseconds);
        }
    }

    /// <summary>
    /// Runs the same helpers upload-media uses (IsImage, IsGifAnimated, thumbnail resize, JPEG compress) on a generated PNG.
    /// CPU-bound, so it goes to the thread pool instead of the startup thread.
    /// </summary>
    private static Task WarmImagePipeline(CancellationToken ct)
    {
        return Task.Run(() =>
        {
            using var png = new MemoryStream();
            using (var image = new MagickImage(MagickColors.White, 512, 512))
            {
                image.Format = MagickFormat.Png;
                image.Write(png);
            }
            png.Position = 0;

            var file = new FormFile(png, 0, png.Length, "file", "warmup.png");
            _ = file.OpenReadStream().IsImage();
            _ = file.IsGifAnimated();
            _ = file.CompressAndConvertToJpeg(288, 432, 100);
            _ = file.CompressAndConvertToJpeg(80);
        }, ct);
    }

    /// <summary>
    /// Opens the connection pool and compiles the queries upload-media runs before touching MinIO
    /// </summary>
    private async Task WarmDatabase(CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<IMcsgContext>();

        await context.GetSettingDouble("ThumbnailCoverSize");
        await context.UserAvailable.AsNoTracking().FirstOrDefaultAsync(p => p.Id == Guid.Empty, ct);
        await context.ComicResources.AsNoTracking().FirstOrDefaultAsync(p => p.HashId == "warmup", ct);
    }

    /// <summary>
    /// A StatObject on a missing key is enough to build the client, do the TLS handshake and cache the bucket region
    /// </summary>
    private async Task WarmMinio(CancellationToken ct)
    {
        foreach (var instance in new[] { MinioInstanceType.Default, MinioInstanceType.Comic })
        {
            try
            {
                await _storageClient.GetStrategy(instance).StatObject("warmup/does-not-exist", null);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "[WARMUP] minio instance {Instance} not warmed", instance);
            }
        }
    }

    #endregion

    #region -- Fields --

    /// <summary>
    /// Upper bound per step; MinIO's HttpClient default is 100s and EF retry can add ~2 min, far too long to hold readiness
    /// </summary>
    private static readonly TimeSpan StepTimeout = TimeSpan.FromSeconds(15);

    private readonly IServiceScopeFactory _scopeFactory;

    private readonly IStorageClient _storageClient;

    private readonly ILogger<StartupWarmupHostedService> _logger;

    #endregion
}

/// <summary>
/// Flipped once by StartupWarmupHostedService; read by the readiness health check
/// </summary>
public static class WarmupState
{
    public static bool IsReady => _isReady;

    public static void MarkReady() => _isReady = true;

    private static volatile bool _isReady;
}
