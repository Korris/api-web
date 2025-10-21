using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Mcsg.Media.Tool;

using Common.Core.Extensions;
using Common.Core.Interfaces;
using Common.Domain;
using Common.Domain.Entities;
using Common.SeedWork.Extensions;
using Interfaces;
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
        var builder = WebApplication.CreateBuilder(args);

        // Get assembly name
        var me = typeof(Program);
        var assembly = me.Assembly.GetName().Name;

        // Load settings from the environment
        var st = _prefix.ConvertEnvironmentVariable<Setting>(CommonPrefix);
        st.Prefix = _prefix;

        // Load connection string and additional settings
        var config = new ConfigurationBuilder().AddConfiguration(builder.Configuration).Build();
        st.DefaultConnection = config.GetConnectionString("McsgConnectionString");
        Console.WriteLine($"[DATABASE CONNECTION] {st.DefaultConnection}");
        Console.WriteLine($"{st.AppName} - v{st.AppVersion}");

        // Cấu hình Serilog
        builder.Host.UseSerilog();
        assembly!.StartLogger();
        builder.Services.AddSingleton(Log.Logger);
        builder.Services.AddSingleton<ISetting>(st);

        // DbContext
        builder.Services.AddDbContext<McsgContext>(options => options.UseNpgsql(st.DefaultConnection, x => x.MigrationsAssembly(assembly).EnableRetryOnFailure()), ServiceLifetime.Scoped);
        builder.Services.AddScoped<IMcsgContext, McsgContext>();

        var serviceProvider = builder.Services.BuildServiceProvider();
        var sc = serviceProvider.GetService<IStorageClient>();

        #region -- Load settings --
        using (var ss = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
        {
            var context = ss.ServiceProvider.GetRequiredService<IMcsgContext>();
            var systemSettings = context.SystemSettings.Where(p => !string.IsNullOrWhiteSpace(p.Key))
                .Select(p => new SystemSetting
                {
                    Key = p.Key,
                    Value = p.Value,
                    DataType = p.DataType
                })
                .ToList();

            // Load config from database
            var configs = context.SystemConfigs.Where(p => !string.IsNullOrWhiteSpace(p.Key)).ToList();

            LoadSettings.LoadSettingsFromDatabase(st, configs);
            builder.Services.AddStorage(p => { p.Storages = st.Minio.Storages; });

            var set = systemSettings.ToDictionary(p => p.Key + "", p => p);
            if (set.TryGetValue("XApiKey", out var ett)) Setting.XApiKey = ett.Value.Cast<string?>(ett.DataType) ?? "";

            var dic = systemSettings.ToDictionary(p => p.Key + "", p => p.Value + "");
            st.LoadApiUrl(dic, st.IsLocal, !string.IsNullOrWhiteSpace(st.Protocols));
            st.LoadRpcUrl(dic, st.IsLocal);

            var cancellation = new CancellationTokenSource();
            await new WorkDistributor(context, st, sc!).Run(cancellation.Token);
        }

        Setting.DevelopmentMode = st.DevMode;
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
