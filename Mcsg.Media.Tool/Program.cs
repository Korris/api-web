using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Mcsg.Media.Tool;

using Common.Core.Extensions;
using Common.Core.Interfaces;
using Common.Domain;
using Common.SeedWork.Extensions;
using static Common.SeedWork.Constants.Setting;

/// <summary>
/// Program
/// </summary>
internal class Program
{
    #region -- Methods --

    /// <summary>
    /// Main
    /// </summary>
    /// <param name="args">Arguments</param>
    static async Task Main(string[] args)
    {
        var configurationBuilder = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddEnvironmentVariables()
            .AddCommandLine(args);

        // Get assembly name
        var me = typeof(Program);
        var assembly = me.Assembly.GetName().Name;

        // Load settings from the environment
        var st = _prefix.ConvertEnvironmentVariable<Setting>(CommonPrefix);
        st.Prefix = _prefix;

        // Update connection string
        st.DefaultConnection = st.DefaultConnection.SetDbParams(st.Db);

        Console.WriteLine($"{st.AppName} - v{st.AppVersion}");

        var builder = new HostBuilder();
        var services = new ServiceCollection();

        // Start logger
        builder.UseSerilog();
        assembly!.StartLogger();
        services.AddSingleton(Log.Logger);

        // DbContext
        services.AddDbContext<McsgContext>(p => p.UseNpgsql(st.DefaultConnection, p => p.MigrationsAssembly(assembly).EnableRetryOnFailure()), ServiceLifetime.Scoped);
        services.AddScoped<IMcsgContext>(p => p.GetService<McsgContext>()!);

        // Storage
        st.LoadStorages();
        services.AddStorage(p => { p.Storages = st.Minio.Storages; });

        var serviceProvider = services.BuildServiceProvider();
        var sc = serviceProvider.GetService<IStorageClient>();

        #region -- Load settings --
        using (var ss = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
        {
            var context = ss.ServiceProvider.GetRequiredService<IMcsgContext>();
            var dic = context.SystemSettings.Where(p => !string.IsNullOrWhiteSpace(p.Key)).ToDictionary(p => p.Key + "", p => p.Value + "");

            st.LoadApiUrl(dic, st.IsLocal, !string.IsNullOrWhiteSpace(st.Protocols));

            var cancellation = new CancellationTokenSource();
            await new WorkDistributor(context, st, sc!).Run(cancellation.Token);
        }
        st.LogInfor();
        #endregion

        Console.ReadLine();
    }

    #endregion

    #region -- Fields --

    /// <summary>
    /// Variable prefix
    /// </summary>
    private static string _prefix = "Med";

    #endregion
}
