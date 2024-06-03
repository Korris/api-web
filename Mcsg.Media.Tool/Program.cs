// See https://aka.ms/new-console-template for more information
using Mcsg.Media.Tool;
using Microsoft.Extensions.Configuration;

var configurationBuilder = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddXmlFile("appsettings.xml", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .AddCommandLine(args);

IConfiguration configuration = configurationBuilder.Build();

string appName = configuration["AppSettings:AppName"];
string appVersion = configuration["AppSettings:AppVersion"];
Console.WriteLine($"{appName} - v{appVersion}");

await new WorkDistributor(configuration).Run();
Console.ReadLine();