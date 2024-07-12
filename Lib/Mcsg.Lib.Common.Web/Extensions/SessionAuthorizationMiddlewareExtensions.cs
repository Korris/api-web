using Microsoft.AspNetCore.Builder;

namespace Mcsg.Lib.Common.Web.Extensions;

using Middlewares;

public static class SessionAuthorizationMiddlewareExtensions
{
    public static IApplicationBuilder UserSessionAuthorizationMiddleware(this WebApplication applicationBuilder)
        => applicationBuilder.UseMiddleware<SessionAuthorizationMiddleware>();
}
