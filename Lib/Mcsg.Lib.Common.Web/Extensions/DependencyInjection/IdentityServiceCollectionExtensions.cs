using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Mcsg.Lib.Common.Web.Extensions.DependencyInjection;

using Mcsg.Common.Domain;
using Mcsg.Common.Domain.Entities;

public static class IdentityServiceCollectionExtensions
{
    public static IServiceCollection AddIdentity<TErrorDescriber>(this IServiceCollection services) where TErrorDescriber : IdentityErrorDescriber
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
        .AddEntityFrameworkStores<McsgDbContext>()
        .AddDefaultTokenProviders()
        .AddUserManager<ApplicationUserManager>()
        .AddErrorDescriber<TErrorDescriber>();

        return services;
    }

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
        .AddEntityFrameworkStores<McsgDbContext>()
        .AddDefaultTokenProviders();

        return services;
    }
}
