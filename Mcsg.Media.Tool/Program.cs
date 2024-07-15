using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Mcsg.Media.Tool;

using Common.Core.Extensions;
using Common.Core.Interfaces;
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

        // Load settings from the environment
        var st = _prefix.ConvertEnvironmentVariable<Setting>(CommonPrefix);
        st.Prefix = _prefix;

        // Update connection string
        st.DefaultConnection = st.DefaultConnection.SetDbParams(st.Db);

        Console.WriteLine($"{st.AppName} - v{st.AppVersion}");

        var services = new ServiceCollection();

        // Storage
        services.AddStorage(p =>
        {
            p.BucketName = st.Minio.BucketName;
            p.Location = st.Minio.Location;
            p.EndPoint = st.Minio.EndPoint;
            p.PublicUrl = st.Minio.PublicUrl;
            p.AccessKey = st.Minio.AccessKey;
            p.SecrectKey = st.Minio.SecrectKey;
        });

        var serviceProvider = services.BuildServiceProvider();
        var sc = serviceProvider.GetService<IStorageClient>();

        var cancellation = new CancellationTokenSource();
        await new WorkDistributor(st, sc!).Run(cancellation.Token);
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
