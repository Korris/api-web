using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Mcsg.Lib.Data.Wallet;

/// <summary>
/// WalletDbContextFactory
/// </summary>
public class WalletDbContextFactory : IDesignTimeDbContextFactory<WalletDbContext>
{
    /// <summary>
    /// CreateDbContext
    /// </summary>
    /// <param name="args">Arguments</param>
    /// <returns>Return the result</returns>
    public WalletDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<WalletDbContext>();
        optionsBuilder.UseNpgsql(args[0]); // args[0] as a connection string

        return new WalletDbContext(optionsBuilder.Options);
    }
}
