using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using System.Data;

namespace Mcsg.Common.Domain.Extensions;

using Domain;
using Domain.Entities;
using Lib.Data.Repositories;
using Lib.Data.Repositories.Interface;

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

    /// <summary>
    /// AddDataLibrary
    /// </summary>
    /// <param name="services">Services</param>
    /// <param name="connectionString">Connection string</param>
    /// <returns>Return the result</returns>
    public static IServiceCollection AddDataLibrary(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<McsgContext>(options =>
        {
            options.UseNpgsql(connectionString,
                builder =>
                {
                    builder.MigrationsAssembly(typeof(McsgContext).Assembly.FullName);
                    builder.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                    builder.EnableRetryOnFailure();
                });

            options.UseOpenIddict();
        });

        services.AddScoped<IMcsgContext>(p => p.GetService<McsgContext>()!);
        services.AddScoped<IDbConnection>((sp) => new NpgsqlConnection(connectionString));
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }

    #endregion
}
