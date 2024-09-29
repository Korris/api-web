using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using System.Data;

namespace Mcsg.Common.Core.Extensions;

using Domain;
using Mcsg.Lib.Data.Repositories;
using Mcsg.Lib.Data.Repositories.Interface;

public static class DependencyInjection
{
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
}