using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Mcsg.Lib.Data.Wallet
{
    public class WalletDbContextFactory : IDesignTimeDbContextFactory<WalletDbContext>
    {
        public WalletDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<WalletDbContext>();
            optionsBuilder.UseSqlServer(args[0]); // args[0] as a connection string

            return new WalletDbContext(optionsBuilder.Options);
        }
    }
}
