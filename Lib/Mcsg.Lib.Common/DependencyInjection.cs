using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Mcsg.Lib.Common;

using Distributor;
using Mail;

public static class DependencyInjection
{
    public static IServiceCollection AddCommonLibrary(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IEmailSender, SmtpSender>();
        return services;
    }

    public static IServiceCollection AddDistributionLibrary(this IServiceCollection services, Assembly assembly)
    {
        services.AddSingleton((s) =>
        {
            return new DistributeManager(assembly, services.BuildServiceProvider());
        });
        return services;
    }
}
