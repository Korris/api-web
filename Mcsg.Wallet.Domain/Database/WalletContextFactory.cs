using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Mcsg.Wallet.Domain;

/// <summary>
/// WalletDbContextFactory
/// </summary>
public class WalletContextFactory : IDesignTimeDbContextFactory<WalletContext>
{
    /// <summary>
    /// CreateDbContext
    /// </summary>
    /// <param name="args">Arguments</param>
    /// <returns>Return the result</returns>
    public WalletContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<WalletContext>();
        optionsBuilder.UseNpgsql(args[0]); // args[0] as a connection string

        return new WalletContext(optionsBuilder.Options);
    }
}
