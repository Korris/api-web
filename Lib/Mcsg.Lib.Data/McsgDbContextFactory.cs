using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Mcsg.Lib.Data;

/// <summary>
/// McsgDbContextFactory
/// </summary>
public class McsgDbContextFactory : IDesignTimeDbContextFactory<McsgDbContext>
{
    /// <summary>
    /// CreateDbContext
    /// </summary>
    /// <param name="args">Arguments</param>
    /// <returns>Return the result</returns>
    public McsgDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<McsgDbContext>();
        optionsBuilder.UseNpgsql(args[0]); // args[0] as a connection string

        return new McsgDbContext(optionsBuilder.Options);
    }
}
