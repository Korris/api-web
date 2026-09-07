namespace Mcsg.Api.Middlewares;

using System.Diagnostics;

/// <summary>
/// Splits the upload timeline: marks when request headers reached the pod, logs pod-side total for large bodies,
/// and emits a Server-Timing header so browser DevTools shows "app" time next to the full round-trip.
/// Together with nginx ingress request_time/upstream_response_time this isolates client, Cloudflare, ingress and app.
/// </summary>
public class RequestTimingMiddleware
{
    #region -- Methods --

    public RequestTimingMiddleware(RequestDelegate next, ILogger<RequestTimingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var startTimestamp = Stopwatch.GetTimestamp();
        context.Items[StartTimestampKey] = startTimestamp;

        // Header must be added before the response starts streaming
        context.Response.OnStarting(() =>
        {
            var ms = Stopwatch.GetElapsedTime(startTimestamp).TotalMilliseconds;
            context.Response.Headers["Server-Timing"] = $"app;dur={ms:F0}";
            // Web app is on another origin; without this the browser hides Server-Timing in DevTools
            context.Response.Headers["Timing-Allow-Origin"] = "*";
            return Task.CompletedTask;
        });

        try
        {
            await _next(context);
        }
        finally
        {
            var contentLength = context.Request.ContentLength ?? 0;
            if (contentLength >= LargeBodyBytes)
            {
                _logger.LogInformation("[REQ-TIMING] {Method} {Path} bodyBytes={Bytes} status={Status} podTotal={Ms}ms",
                    context.Request.Method, context.Request.Path, contentLength, context.Response.StatusCode,
                    Stopwatch.GetElapsedTime(startTimestamp).TotalMilliseconds.ToString("F0"));
            }
        }
    }

    /// <summary>
    /// Milliseconds since request headers reached this pod, for logging inside actions (e.g. after the multipart body was read)
    /// </summary>
    public static double ElapsedSinceRequestStart(HttpContext context)
    {
        return context.Items.TryGetValue(StartTimestampKey, out var value) && value is long start
            ? Stopwatch.GetElapsedTime(start).TotalMilliseconds
            : 0;
    }

    #endregion

    #region -- Fields --

    private const string StartTimestampKey = "__RequestStartTimestamp";

    /// <summary>
    /// Only log pod-side totals for bodies at least this big (uploads); keeps normal API traffic out of the log
    /// </summary>
    private const long LargeBodyBytes = 256 * 1024;

    private readonly RequestDelegate _next;

    private readonly ILogger<RequestTimingMiddleware> _logger;

    #endregion
}
