using Mcsg.Lib.Common.Web.Middlewares;
using Microsoft.AspNetCore.Builder;

namespace Mcsg.Lib.Common.Web.Extensions
{
    public static class ApiResponseAndExceptionWrapperExtensions
    {
        public static IApplicationBuilder UseApiResponseAndExceptionWrapper(this WebApplication applicationBuilder) => applicationBuilder.UseMiddleware<ApiResponseAndExceptionWrapperMiddleware>();
    }
}
