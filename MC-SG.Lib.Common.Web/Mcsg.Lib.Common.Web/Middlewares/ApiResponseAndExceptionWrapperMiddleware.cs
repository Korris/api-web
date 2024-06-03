using Mcsg.Lib.Common.Exceptions;
using Mcsg.Lib.Common.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Mcsg.Lib.Common.Web.Middlewares
{
    public class ApiResponseAndExceptionWrapperMiddleware
    {
        private readonly RequestDelegate _next;

        public ApiResponseAndExceptionWrapperMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (SkipApiResponseMiddleware(context))
            {
                await _next(context);
            }
            else
            {
                var originalBodyStream = context.Response.Body;

                using (var responseBody = new MemoryStream())
                {
                    context.Response.Body = responseBody;

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
                        await PackageResponse(originalBodyStream, responseBody);
                    }
                }
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception error)
        {
            HttpResponse response = context.Response;
            HttpStatusCode status = error switch
            {
                ForbiddenAccessException => HttpStatusCode.Forbidden,
                BadRequestException => HttpStatusCode.BadRequest,
                NotFoundException => HttpStatusCode.NotFound,
                AppUnauthorizedAccessException => HttpStatusCode.Unauthorized,
                _ => HttpStatusCode.InternalServerError,
            };

            //TODO: logging exception error if need

            response.StatusCode = (int)status;

            await response.WriteAsJsonAsync(new ApiResponse
            {
                Status = status.ToString(),
                Error = new ApiErrorResponse()
                {
                    Code = (error as BaseException)?.Code,
                    Message = error.Message,
                },
                Path = context.Request.Path
            });
        }

        private async Task PackageResponse(Stream originalBodyStream, MemoryStream responseBody)
        {
            responseBody.Seek(0, SeekOrigin.Begin);
            await responseBody.CopyToAsync(originalBodyStream);
        }

        private bool SkipApiResponseMiddleware(HttpContext context)
        {
            return IsSwagger(context) || context.Request.Method == HttpMethods.Options;
        }

        private bool IsSwagger(HttpContext context)
        {
            return context.Request.Path.StartsWithSegments("/swagger");
        }

        private async Task HandleRequestAsync(HttpContext context)
        {
            var body = await FormatResponse(context.Response);

            await HandleRequestAsync(context, body);
        }

        private async Task HandleRequestAsync(HttpContext context, object body)
        {
            var code = context.Response.StatusCode;

            if (body != null)
            {
                context.Response.Body.SetLength(0L);
                var bodyString = body.ToString();
                dynamic data = bodyString;

                if (IsValidJson(bodyString))
                {
                    data = JsonNode.Parse(bodyString);

                }

                await context.Response.WriteAsJsonAsync(new ApiResponse
                {
                    Status = code.ToString(),
                    Data = data,
                    Path = context.Request.Path
                });
            }
        }

        static bool IsValidJson(string jsonString)
        {
            try
            {
                JsonDocument.Parse(jsonString);
                return true;
            }
            catch (JsonException)
            {
                return false;
            }
        }

        private async Task<string> FormatResponse(HttpResponse response)
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            var plainBodyText = await new StreamReader(response.Body).ReadToEndAsync();
            response.Body.Seek(0, SeekOrigin.Begin);

            return plainBodyText;
        }
    }
}
