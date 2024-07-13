using Microsoft.Extensions.DependencyInjection;

namespace Mcsg.Lib.Common.Web.Extensions;

using Security;

/// <summary>
/// IServiceCollection extension for using [this IServiceCollection] only
/// </summary>
public static class IServiceCollectionExtension
{
    /// <summary>
    /// Add common web library
    /// </summary>
    /// <param name="service">Service</param>
    /// <returns></returns>
    public static IServiceCollection AddCommonWebLibrary(this IServiceCollection service)
    {
        service.AddSingleton<ICurrentUserService, CurrentUserService>();
        service.AddHttpContextAccessor();

        service.AddScoped<ISecurityService, SecurityService>();
        return service;
    }
}
