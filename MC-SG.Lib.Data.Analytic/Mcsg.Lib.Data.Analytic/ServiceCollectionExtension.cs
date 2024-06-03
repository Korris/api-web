using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using System.Data;

namespace Mcsg.Lib.Data.Analytic
{
    public static class ServiceCollectionExtension
    {
        public static void AddAnalyticDbContext(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<AnalyticDbContext>(options =>
            {
                options.UseNpgsql(connectionString,
                    builder =>
                    {
                        builder.MigrationsAssembly(typeof(AnalyticDbContext).Assembly.FullName);
                        builder.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                        builder.EnableRetryOnFailure();
                    })
                ;
            });

            services.AddScoped<IDbConnection>((sp) => new NpgsqlConnection(connectionString));
        }

        public static void AddAnalyticDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AnalyticDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("AnalyticDbConnectionString"),
                    builder =>
                    {
                        builder.MigrationsAssembly(typeof(AnalyticDbContext).Assembly.FullName);
                        builder.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                        builder.EnableRetryOnFailure();
                    })
                ;
            });
            services.AddScoped<IDbConnection>((sp) => new NpgsqlConnection(configuration.GetConnectionString("AnalyticDbConnectionString")));
        }
    }
}
