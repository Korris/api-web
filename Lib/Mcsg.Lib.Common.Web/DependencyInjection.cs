using MC_SG.Lib.Common.Web.Security;
using Mcsg.Lib.Common.Web.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MC_SG.Lib.Common.Web
{
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
}
