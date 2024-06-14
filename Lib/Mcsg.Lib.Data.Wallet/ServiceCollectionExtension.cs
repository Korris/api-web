using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Mcsg.Lib.Data.Wallet
{
    public static class ServiceCollectionExtension
    {
        public static void AddWalletDbContext(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<WalletDbContext>(o =>
            {
                o.UseNpgsql(connectionString,
                    options => options.EnableRetryOnFailure());
            });
        }

        public static void AddWalletDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<WalletDbContext>(o =>
            {
                o.UseNpgsql(configuration.GetConnectionString("WalletDbConnectionString"),
                    options => options.EnableRetryOnFailure());
            });
        }
    }
}
