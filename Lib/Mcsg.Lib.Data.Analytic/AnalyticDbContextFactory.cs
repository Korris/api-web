using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Mcsg.Lib.Data.Analytic;

/// <summary>
/// AnalyticDbContextFactory
/// </summary>
public class AnalyticDbContextFactory : IDesignTimeDbContextFactory<AnalyticDbContext>
{
    /// <summary>
    /// CreateDbContext
    /// </summary>
    /// <param name="args">Arguments</param>
    /// <returns>Return the result</returns>
    public AnalyticDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AnalyticDbContext>();
        optionsBuilder.UseNpgsql(args[0]); // args[0] as a connection string

        return new AnalyticDbContext(optionsBuilder.Options);
    }
}
