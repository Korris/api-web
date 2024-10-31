using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;

namespace Mcsg.Wallet.Job;

using Common.Core.Extensions;
using Common.SeedWork;
using Common.SeedWork.Extensions;
using Domain;
using Domain.Entities;
using Domain.Interfaces;
using Extensions;
using Interfaces;
using Lib.Common.Mail;
using Lib.Common.Models;
using Services;
using static Common.Core.Constants.Setting;
using static Common.SeedWork.Constants.Setting;

/// <summary>
/// Program
/// </summary>
public class Program
{
    #region -- Methods --

    /// <summary>
    /// Main
    /// </summary>
    /// <param name="args">Arguments</param>
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddHealthChecks();

        // Get assembly name
        var me = typeof(Program);
        var assembly = me.Assembly.GetName().Name;

        // Load settings from the environment
        var st = _prefix.ConvertEnvironmentVariable<Setting>(CommonPrefix);
        st.Prefix = _prefix;

        // Load connection string appsettings.json
        var config = new ConfigurationBuilder().AddConfiguration(builder.Configuration).Build();
        var cs = config.GetConnectionString("McsgConnectionString");

        #region -- Load settings --
        config.LoadSettings(st, "Queue:Notification");
        #endregion

        // Update connection string
        var csDb = cs.SetDbParams(st.Db);

        // Start logger
        builder.Host.UseSerilog();
        assembly!.StartLogger();
        builder.Services.AddSingleton(Log.Logger);

        #region -- Load HTTP protocols --
        if (!string.IsNullOrWhiteSpace(st.Protocols))
        {
            var protocols = st.Protocols.Split(';', StringSplitOptions.RemoveEmptyEntries);

            builder.WebHost.ConfigureKestrel(p =>
            {
                foreach (var i in protocols)
                {
                    var arr = i.Split('_', StringSplitOptions.RemoveEmptyEntries);
                    if (arr.Length != 2)
                    {
                        continue;
                    }

                    var port = Convert.ToInt32(arr[1]);
                    var protocol = HttpProtocols.Http1;

                    if (nameof(HttpProtocols.Http2) == arr[0])
                    {
                        protocol = HttpProtocols.Http2;
                    }

                    p.ListenAnyIP(port, q => q.Protocols = protocol);
                }
            });
        }
        #endregion

        #region -- Setup DI --
        // Setting
        builder.Services.AddSingleton<ISetting>(st!);

        // SecurityAes
        builder.Services.AddSingleton<ISecurityAes>(p => new SecurityAes(st.EncryptKey));

        // DbContext
        builder.Services.AddWalletDbContext(csDb);

        // Notification sent via email (using SMTP)
        builder.Services.AddNotification(p =>
        {
            p.Host = st.Email.Host;
            p.Port = st.Email.Port;
            p.UserName = st.Email.UserName;
            p.Password = st.Email.Password;
            p.SenderEmail = st.Email.SenderEmail;
            p.SenderName = st.Email.SenderName;
        });
        builder.Services.Configure<SmtpSettings>(p =>
        {
            p.SmtpHost = st.Email.Host;
            p.SmtpPort = st.Email.Port;
            p.SmtpUser = st.Email.UserName;
            p.SmtpPass = st.Email.Password;
            p.SmtpFrom = st.Email.SenderEmail;
            p.SmtpDisplayFrom = st.Email.SenderName;
        });

        // Storage
        st.LoadStorages();
        builder.Services.AddStorage(p => { p.Storages = st.Minio.Storages; });

        // Service
        builder.Services.AddScoped<IEmailService, EmailService>();
        builder.Services.AddSingleton<IEmailSender, SmtpSender>();
        //builder.Services.AddScoped<ISyncDataService, SyncDataService>();
        builder.Services.AddScoped<IPaymentService, PaymentService>();
        #endregion

        #region -- Setup token --
        builder.Services.AddBearerAuthentication(st.Jwt);
        builder.Services.AddResponseCaching();

        // Cookie name
        builder.Services.ConfigureApplicationCookie(p => { p.Cookie.Name = _prefix; });
        #endregion

        builder.Services.AddControllers();

        // AddHostedService
        builder.Services.AddHostedService<HostedEmail>();
        builder.Services.AddHostedService<HostedPaymentTransaction>();
        //builder.Services.AddHostedService<HostedSyncData>();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(p => { p.EnableAnnotations(); });

        builder.Services.ConfigureMaxRequestSizes();

        var app = builder.Build();

        // https://stackoverflow.com/questions/69961449/net6-and-datetime-problem-cannot-write-datetime-with-kind-utc-to-postgresql-ty
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        #region -- Load settings --
        using (var ss = app.Services.GetService<IServiceScopeFactory>()!.CreateScope())
        {
            var context = ss.ServiceProvider.GetRequiredService<IWalletContext>();
            var systemSettings = context.SystemSettings.Where(p => !string.IsNullOrWhiteSpace(p.Key))
                .Select(p => new SystemSetting
                {
                    Key = p.Key,
                    Value = p.Value,
                    DataType = p.DataType
                })
                .ToList();

            var set = systemSettings.ToDictionary(p => p.Key + "", p => p);
            if (set.TryGetValue("XApiKey", out var ett)) Setting.XApiKey = ett.Value.Cast<string?>("string") ?? "";

            var dic = systemSettings.ToDictionary(p => p.Key + "", p => p.Value + "");
            st.LoadApiUrl(dic, st.IsLocal, !string.IsNullOrWhiteSpace(st.Protocols));
            st.LoadRpcUrl(dic, st.IsLocal);
        }

        Setting.DevelopmentMode = st.DevMode;
        st.LogInfor();
        #endregion

        #region -- Swagger and CORS --
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment() || st.SwaggerEnabled)
        {
            if (st.Environment == "local")
            {
                app.UseSwagger();
            }
            else
            {
                app.UseSwagger(p =>
                {
                    p.RouteTemplate = "swagger/{documentName}/swagger.json";
                    p.PreSerializeFilters.Add((q, r) =>
                    {
                        q.Servers = [new OpenApiServer { Url = $"{st.Domain}/api/{MicroServices.GetValueOrDefault(_prefix)}".ToLower() }];
                    });
                });
            }

            app.UseSwaggerUI();
        }
        else
        {
            app.UseDeveloperExceptionPage();
        }

        // Use CORS
        var origins = st.Origins == null ? [] : st.Origins.Split(';');
        if (origins.Length > 0)
        {
            app.UseCors(p => p.AllowAnyHeader().AllowAnyMethod().WithOrigins(origins).AllowCredentials());
        }
        #endregion

        app.UseHttpsRedirection();
        app.UseAuthorization();

        app.MapControllers();
        app.MapHealthChecks("/health");
        app.UseResponseCaching();

        app.Run();
    }

    #endregion

    #region -- Fields --

    /// <summary>
    /// Variable prefix
    /// </summary>
    private static string _prefix = "Wjo";

    #endregion
}
