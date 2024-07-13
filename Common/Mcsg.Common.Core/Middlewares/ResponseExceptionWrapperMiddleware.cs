using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Mcsg.Common.Core.Middlewares;

using Dtos;
using SeedWork.Exceptions;

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
        if (SkipApiResponseMiddleware(context))
        {
            await _next(context);
        }
        else
        {
            var body = context.Response.Body;
            using (var ms = new MemoryStream())
            {
                context.Response.Body = ms;

                try
                {
                    await _next.Invoke(context);
                    await HandleRequestAsync(context);
                }
                catch (Exception ex)
                {
                    if (ex.Message == "No password has been provided but the backend requires one (in MD5)")
                    {
                        await _next.Invoke(context);
                    }
                    else
                    {
                        await HandleExceptionAsync(context, ex);
                    }
                }
                finally
                {
                    await PackageResponse(body, ms);
                }
            }
        }
    }

    /// <summary>
    /// Handle exception async
    /// </summary>
    /// <param name="context">HTTP context</param>
    /// <param name="error">Error</param>
    /// <returns>Return the result</returns>
    private async Task HandleExceptionAsync(HttpContext context, Exception error)
    {
        var response = context.Response;
        var status = error switch
        {
            ForbiddenAccessException => HttpStatusCode.Forbidden,
            BadRequestException => HttpStatusCode.BadRequest,
            NotFoundException => HttpStatusCode.NotFound,
            UnauthorizedAccessException => HttpStatusCode.Unauthorized,
            _ => HttpStatusCode.InternalServerError,
        };

        //TODO: logging exception error if need
        response.StatusCode = (int)status;

        await response.WriteAsJsonAsync(new ApiDataDto
        {
            Status = status.ToString(),
            Error = new ApiErrorDto()
            {
                Code = (error as BaseException)?.Code,
                Message = error.Message,
            },
            Path = context.Request.Path
        });
    }

    /// <summary>
    /// Package response
    /// </summary>
    /// <param name="originalBody">Original body</param>
    /// <param name="responseBody">Response body</param>
    /// <returns>Return the result</returns>
    private async Task PackageResponse(Stream originalBody, MemoryStream responseBody)
    {
        responseBody.Seek(0, SeekOrigin.Begin);
        await responseBody.CopyToAsync(originalBody);
    }

    /// <summary>
    /// Skip API response middleware
    /// </summary>
    /// <param name="context">HTTP context</param>
    /// <returns>Return the result</returns>
    private bool SkipApiResponseMiddleware(HttpContext context)
    {
        return IsSwagger(context) || context.Request.Method == HttpMethods.Options;
    }

    /// <summary>
    /// Is Swagger
    /// </summary>
    /// <param name="context">HTTP context</param>
    /// <returns>Return the result</returns>
    private bool IsSwagger(HttpContext context)
    {
        return context.Request.Path.StartsWithSegments("/swagger");
    }

    /// <summary>
    /// Handle request async
    /// </summary>
    /// <param name="context">HTTP context</param>
    /// <returns>Return the result</returns>
    private async Task HandleRequestAsync(HttpContext context)
    {
        var body = await FormatResponse(context.Response);
        await HandleRequestAsync(context, body);
    }

    /// <summary>
    /// Handle request async
    /// </summary>
    /// <param name="context">HTTP context</param>
    /// <param name="body"></param>
    /// <returns>Return the result</returns>
    private async Task HandleRequestAsync(HttpContext context, object body)
    {
        var code = context.Response.StatusCode;

        if (body != null)
        {
            context.Response.Body.SetLength(0L);
            var bodyString = body.ToString() + "";
            dynamic? data = bodyString;

            if (IsValidJson(bodyString))
            {
                data = JsonNode.Parse(bodyString);
            }

            await context.Response.WriteAsJsonAsync(new ApiDataDto
            {
                Status = code.ToString(),
                Data = data,
                Path = context.Request.Path
            });
        }
    }

    /// <summary>
    /// Is valid JSON
    /// </summary>
    /// <param name="json">JSON string</param>
    /// <returns>Return the result</returns>
    static bool IsValidJson(string json)
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

    /// <summary>
    /// Format response
    /// </summary>
    /// <param name="response">Response</param>
    /// <returns>Return the result</returns>
    private async Task<string> FormatResponse(HttpResponse response)
    {
        response.Body.Seek(0, SeekOrigin.Begin);
        var res = await new StreamReader(response.Body).ReadToEndAsync();
        response.Body.Seek(0, SeekOrigin.Begin);
        return res;
    }

    #endregion

    #region -- Fields --

    /// <summary>
    /// Request delegate
    /// </summary>
    private readonly RequestDelegate _next;

    #endregion
}
