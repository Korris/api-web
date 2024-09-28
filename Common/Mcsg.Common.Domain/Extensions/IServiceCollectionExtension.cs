using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Mcsg.Common.Domain.Extensions;

using Domain;
using Domain.Entities;

/// <summary>
/// IServiceCollection extension for using [this IServiceCollection] only
/// </summary>
public static class IServiceCollectionExtension
{
    #region -- Methods --

    /// <summary>
    /// AddIdentity
    /// </summary>
    /// <typeparam name="T">Error describer</typeparam>
    /// <param name="services">Services</param>
    /// <returns>Return the result</returns>
    public static IServiceCollection AddIdentity<T>(this IServiceCollection services) where T : IdentityErrorDescriber
    {
        services.AddIdentityCore<User>(option =>
        {
            option.Password.RequireDigit = true;
            option.Password.RequireLowercase = true;
            option.Password.RequireNonAlphanumeric = true;
            option.Password.RequireUppercase = true;
            option.Password.RequiredLength = 8;
        })
        .AddRoles<Role>()
        .AddEntityFrameworkStores<McsgContext>()
        .AddDefaultTokenProviders()
        .AddUserManager<ApplicationUserManager>()
        .AddErrorDescriber<T>();

        return services;
    }

    /// <summary>
    /// AddIdentity
    /// </summary>
    /// <param name="services">Services</param>
    /// <returns>Return the result</returns>
    public static IServiceCollection AddIdentity(this IServiceCollection services)
    {
        services.AddIdentityCore<User>(option =>
        {
            option.Password.RequireDigit = true;
            option.Password.RequireLowercase = true;
            option.Password.RequireNonAlphanumeric = true;
            option.Password.RequireUppercase = true;
            option.Password.RequiredLength = 8;
        })
        .AddRoles<Role>()
        .AddEntityFrameworkStores<McsgContext>()
        .AddDefaultTokenProviders();

        return services;
    }

    #endregion
}
