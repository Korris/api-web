using Mcsg.Lib.Common.Web.RealTime.Hubs;
using Mcsg.Lib.Common.Web.RealTime.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Mcsg.Lib.Common.Web.Extensions
{
    public static class RealTimeExtensions
    {
        public static IServiceCollection AddSignalR(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ISignalRService, SignalRService>();
            services.AddSignalR();
            return services;
        }
        public static IApplicationBuilder UseCommonHub(this WebApplication applicationBuilder)
        {
            applicationBuilder.MapHub<CommonHub>("/commonHub");
            return applicationBuilder;
        }
    }
}
