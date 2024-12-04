using Grpc.Core;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Mcsg.Common.Core.Middlewares;

using Dtos;
using SeedWork;
using SeedWork.Exceptions;
using static Constants.Setting;

/// <summary>
/// Response and Exception wrapper middleware
/// </summary>
public class ResponseExceptionWrapperMiddleware
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="next">Request delegate</param>
    public ResponseExceptionWrapperMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    /// <summary>
    /// Invoke
    /// </summary>
    /// <param name="context">HTTP context</param>
    /// <returns>Return the result</returns>
    public async Task Invoke(HttpContext context)
    {
        // Skip the middleware logic for gRPC requests
        if (IsGrpcRequest(context))
        {
            await _next(context);
            return;
        }

        // Skip the middleware logic for WebSocket requests
        if (IsWebsocketRequest(context))
        {
            await _next(context);
            return;
        }

        // Skip the middleware logic for XApiKey requests
        if (!IsAuthenticated(context))
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            await context.Response.WriteAsJsonAsync(new { message = "Oops server error..." });
            return;
        }

        // Handle other requests
        if (SkipApiResponseMiddleware(context))
        {
            await _next(context);
        }
        else
        {
            var originalBodyStream = context.Response.Body;
            using (var memoryStream = new MemoryStream())
            {
                context.Response.Body = memoryStream;

                try
                {
                    // Proceed with the request pipeline
                    await _next.Invoke(context);

                    // Handle non-v1 HTTP responses (not gRPC)
                    var isV1 = (context.Request.Path.Value ?? "").Contains("/v1/");
                    if (!isV1)
                    {
                        await HandleRequestAsync(context);
                    }
                }
                catch (Exception ex)
                {
                    // Handle the exception
                    await HandleExceptionAsync(context, ex);
                }
                finally
                {
                    await PackageResponse(originalBodyStream, memoryStream);
                }
            }
        }
    }

    /// <summary>
    /// Handles exceptions and returns appropriate responses
    /// </summary>
    private async Task HandleExceptionAsync(HttpContext context, Exception error)
    {
        var isGrpc = IsGrpcRequest(context);

        if (isGrpc)
        {
            // Handle exceptions for gRPC requests
            var status = StatusCode.Internal;

            if (error is ForbiddenAccessException)
            {
                status = StatusCode.PermissionDenied;
            }
            else if (error is BadRequestException)
            {
                status = StatusCode.InvalidArgument;
            }
            else if (error is NotFoundException)
            {
                status = StatusCode.NotFound;
            }
            else if (error is UnauthorizedAccessException)
            {
                status = StatusCode.Unauthenticated;
            }

            // Throw gRPC-specific RpcException
            throw new RpcException(new Status(status, error.Message));
        }
        else
        {
            // Handle exceptions for HTTP requests
            var response = context.Response;
            var status = error switch
            {
                ForbiddenAccessException => HttpStatusCode.Forbidden,
                BadRequestException => HttpStatusCode.BadRequest,
                NotFoundException => HttpStatusCode.NotFound,
                UnauthorizedAccessException => HttpStatusCode.Unauthorized,
                _ => HttpStatusCode.InternalServerError,
            };

            response.StatusCode = (int)status;

            // Write the exception as JSON response for HTTP requests
            await response.WriteAsJsonAsync(new ApiDataDto
            {
                Status = status.ToString(),
                Error = new ApiErrorDto
                {
                    Code = (error as BaseException)?.Code,
                    Message = error.Message,
                },
                Path = context.Request.Path
            });
        }
    }

    /// <summary>
    /// Wraps and formats the HTTP response body into a JSON object
    /// </summary>
    private async Task HandleRequestAsync(HttpContext context)
    {
        var responseBody = await FormatResponse(context.Response);
        await HandleRequestAsync(context, responseBody);
    }

    /// <summary>
    /// Wraps and formats the HTTP response body into a JSON object
    /// </summary>
    private async Task HandleRequestAsync(HttpContext context, object body)
    {
        var statusCode = context.Response.StatusCode;

        if (body != null)
        {
            context.Response.Body.SetLength(0L); // Clear current body
            var bodyString = body.ToString() ?? string.Empty;
            dynamic? data = bodyString;

            // Check if the response body is valid JSON
            if (IsValidJson(bodyString))
            {
                data = JsonNode.Parse(bodyString);
            }

            // Write the formatted response as JSON
            await context.Response.WriteAsJsonAsync(new ApiDataDto
            {
                Status = statusCode.ToString(),
                Data = data,
                Path = context.Request.Path
            });
        }
    }

    /// <summary>
    /// Utility to detect if the request is a gRPC request
    /// </summary>
    /// <param name="context">HTTP context</param>
    /// <returns>Return the result</returns>
    private bool IsGrpcRequest(HttpContext context)
    {
        return context.Request.ContentType == "application/grpc";
    }

    /// <summary>
    /// Utility to detect if the request is a WebSocket request
    /// </summary>
    /// <param name="context">HTTP context</param>
    /// <returns>Return the result</returns>
    private bool IsWebsocketRequest(HttpContext context)
    {
        return context.Request.Headers["Upgrade"] == "websocket";
    }

    /// <summary>
    /// Utility to detect if the request is a xApiKey request
    /// </summary>
    /// <param name="context">HTTP context</param>
    /// <returns>Return the result</returns>
    private bool IsAuthenticated(HttpContext context)
    {
        var r = context.Request;
        var skipPath = r.Path == "/config" || r.Path == "/health";
        r.Headers.TryGetValue(HeaderKey.XApiKey, out var xApiKey);
        var ok = SecurityAes.Validate(xApiKey, SettingBase.XApiKey);
        return ok || SettingBase.DevelopmentMode || skipPath;
    }

    /// <summary>
    /// Skip the middleware for certain requests like Swagger or HTTP OPTIONS
    /// </summary>
    /// <param name="context">HTTP context</param>
    /// <returns>Return the result</returns>
    private bool SkipApiResponseMiddleware(HttpContext context)
    {
        return IsSwagger(context) || context.Request.Method == HttpMethods.Options;
    }

    /// <summary>
    /// Check if the request is for Swagger documentation
    /// </summary>
    /// <param name="context">HTTP context</param>
    /// <returns>Return the result</returns>
    private bool IsSwagger(HttpContext context)
    {
        return context.Request.Path.StartsWithSegments("/swagger");
    }

    /// <summary>
    /// Copies the memory stream content back to the original response stream
    /// </summary>
    private async Task PackageResponse(Stream originalBody, MemoryStream responseBody)
    {
        responseBody.Seek(0, SeekOrigin.Begin);
        await responseBody.CopyToAsync(originalBody);
    }

    /// <summary>
    /// Formats the response body as a string
    /// </summary>
    private async Task<string> FormatResponse(HttpResponse response)
    {
        response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(response.Body).ReadToEndAsync();
        response.Body.Seek(0, SeekOrigin.Begin);
        return responseBody;
    }

    /// <summary>
    /// Checks if a string is valid JSON
    /// </summary>
    private static bool IsValidJson(string json)
    {
        try
        {
            JsonDocument.Parse(json);
            return true;
        }
        catch (JsonException)
        {
            return false;
        }
    }

    #endregion

    #region -- Fields --

    /// <summary>
    /// Request delegate
    /// </summary>
    private readonly RequestDelegate _next;

    #endregion
}
