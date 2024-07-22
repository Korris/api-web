using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Mcsg.Common.Domain;

/// <summary>
/// McsgContextFactory
/// </summary>
public class McsgContextFactory : IDesignTimeDbContextFactory<McsgContext>
{
    /// <summary>
    /// CreateDbContext
    /// </summary>
    /// <param name="args">Arguments</param>
    /// <returns>Return the result</returns>
    public McsgContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<McsgContext>();
        optionsBuilder.UseNpgsql(args[0]); // args[0] as a connection string

        return new McsgContext(optionsBuilder.Options);
    }
}
