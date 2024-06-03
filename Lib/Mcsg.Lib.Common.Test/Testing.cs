using Microsoft.Extensions.Configuration;

namespace Mcsg.Lib.Common.Test
{
    public class Testing
    {
        public static IConfigurationRoot _configuration = null!;
        [OneTimeSetUp]
        public void RunBeforeAnyTests()
        {
            var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", true, true)
                    .AddJsonFile("appsettings.Test.json", true, true)
                    .AddEnvironmentVariables();

            _configuration = builder.Build();

        }
    }
}
