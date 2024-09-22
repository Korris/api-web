using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Mcsg.Wallet.Domain;

public static class ServiceCollectionExtension
{
    public static void AddWalletDbContext(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<WalletContext>(o =>
        {
            o.UseNpgsql(connectionString, options => options.EnableRetryOnFailure());
        });
    }
}
