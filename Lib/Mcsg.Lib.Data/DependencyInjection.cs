using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using System.Data;

namespace Mcsg.Lib.Data;

using Mcsg.Common.Domain;
using Repositories;
using Repositories.Interface;

public static class DependencyInjection
{
    public static IServiceCollection AddDataLibrary(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<McsgDbContext>(options =>
        {
            options.UseNpgsql(connectionString,
                builder =>
                {
                    builder.MigrationsAssembly(typeof(McsgDbContext).Assembly.FullName);
                    builder.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                    builder.EnableRetryOnFailure();
                })
            ;
        });
        services.AddScoped<IDbConnection>((sp) => new NpgsqlConnection(connectionString));

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }

    public static IApplicationBuilder EnableNpgsqlLegacyTime(this WebApplication applicationBuilder)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        return applicationBuilder;
    }
}