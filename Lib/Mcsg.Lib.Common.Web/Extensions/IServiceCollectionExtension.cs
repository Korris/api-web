using Microsoft.Extensions.DependencyInjection;

namespace Mcsg.Lib.Common.Web.Extensions;

using Constants;
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
    /// <returns>Returns the result</returns>
    public static IServiceCollection AddCommonWebLibrary(this IServiceCollection service)
    {
        service.AddSingleton<ICurrentUserService, CurrentUserService>();
        service.AddHttpContextAccessor();

        service.AddScoped<ISecurityService, SecurityService>();
        return service;
    }

    /// <summary>
    /// AddSwaggerDocumentation
    /// </summary>
    /// <param name="services">Services</param>
    /// <param name="scheme">Scheme</param>
    /// <returns>Returns the result</returns>
    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services, string scheme)
    {
        services.AddSwaggerGen(options =>
        {
            switch (scheme)
            {
                case AuthenticationSchemes.JwtScheme:
                    options.AddJwtSecurity();
                    break;
                case AuthenticationSchemes.ApiKeyScheme:
                    options.AddApiKeySecurity();
                    break;
                default:
                    break;
            }
        });
        return services;
    }
}
