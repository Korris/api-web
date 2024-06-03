using Mcsg.Lib.Common.Web.Middlewares;
using Microsoft.AspNetCore.Builder;

namespace Mcsg.Lib.Common.Web.Extensions
{
    public static class SessionAuthorizationMiddlewareExtensions
    {
        public static IApplicationBuilder UserSessionAuthorizationMiddleware(this WebApplication applicationBuilder)
            => applicationBuilder.UseMiddleware<SessionAuthorizationMiddleware>();
    }
}
