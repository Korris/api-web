using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Mcsg.Lib.Data.Analytic
{
    internal class AnalyticDbContextFactory : IDesignTimeDbContextFactory<AnalyticDbContext>
    {
        public AnalyticDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AnalyticDbContext>();
            optionsBuilder.UseNpgsql(args[0]); // args[0] as a connection string

            return new AnalyticDbContext(optionsBuilder.Options);
        }
    }
}
