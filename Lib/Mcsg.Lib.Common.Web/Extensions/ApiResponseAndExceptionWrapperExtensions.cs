using Microsoft.AspNetCore.Builder;

namespace Mcsg.Lib.Common.Web.Extensions;

using Middlewares;

public static class ApiResponseAndExceptionWrapperExtensions
{
    public static IApplicationBuilder UseApiResponseAndExceptionWrapper(this WebApplication applicationBuilder) => applicationBuilder.UseMiddleware<ApiResponseAndExceptionWrapperMiddleware>();
}
