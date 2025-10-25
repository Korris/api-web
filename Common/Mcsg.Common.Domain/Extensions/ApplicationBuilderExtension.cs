using Microsoft.AspNetCore.Builder;

namespace Mcsg.Common.Domain.Extensions;

public static class ApiPathRewriteExtension
{
    public static IApplicationBuilder UseApiPathRewrite(
       this IApplicationBuilder app,
       string serviceName)
    {
        var lowerServiceName = serviceName?.ToLower() ?? "";
        var prefix = $"/api/{lowerServiceName}/";

        app.Use(async (context, next) =>
        {
            var path = context.Request.Path.Value ?? "";

            if (path.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            {
                context.Request.Path = path.Replace(prefix, "/");
            }

            await next();
        });

        return app;
    }
}