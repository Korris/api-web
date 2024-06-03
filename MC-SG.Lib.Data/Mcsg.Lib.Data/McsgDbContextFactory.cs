using Mcsg.Lib.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Cecce.Lib.Data;

public class McsgDbContextFactory : IDesignTimeDbContextFactory<McsgDbContext>
{
    public McsgDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<McsgDbContext>();
        optionsBuilder.UseNpgsql(args[0]); // args[0] as a connection string

        return new McsgDbContext(optionsBuilder.Options);
    }
}