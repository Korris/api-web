using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Mcsg.Lib.Common.Web;

using Mcsg.Lib.Common.Web.Security;

public static class DependencyInjection
{
    public static IServiceCollection AddCommonWebLibrary(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<ICurrentUserService, CurrentUserService>();
        services.AddHttpContextAccessor();

        services.AddScoped<ISecurityService, SecurityService>();
        return services;
    }
}
