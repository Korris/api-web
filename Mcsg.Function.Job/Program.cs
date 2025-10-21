using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Mcsg.Function.Job;

using Common.Core.Extensions;
using Common.Domain;
using Common.Domain.Entities;
using Common.Domain.Extensions;
using Common.Mail;
using Common.SeedWork;
using Common.SeedWork.Extensions;
using Extensions;
using Interfaces;
using Mcsg.Common.Models;
using Quartz;
using Services;
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

        // https://stackoverflow.com/questions/69961449/net6-and-datetime-problem-cannot-write-datetime-with-kind-utc-to-postgresql-ty
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        // Get assembly name
        var me = typeof(Program);
        var assembly = me.Assembly.GetName().Name;

        // Load settings from the environment
        var st = _prefix.ConvertEnvironmentVariable<Setting>(CommonPrefix);
        st.Prefix = _prefix;

        // Load connection string appsettings.json
        var config = new ConfigurationBuilder().AddConfiguration(builder.Configuration).Build();
        var cs = config.GetConnectionString("McsgConnectionString");
        Console.WriteLine($"[DATABASE CONNECTION] {cs}");
        #region -- Load settings --
        config.LoadSettings(st, "Queue:Notification");

        #endregion

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
        builder.Services.AddDataLibrary(cs);

        #region -- Load settings --
        var serviceProvider = builder.Services.BuildServiceProvider();
        using (var ss = serviceProvider.GetService<IServiceScopeFactory>()!.CreateScope())
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

            // Load config
            var configs = context.SystemConfigs.Where(p => !string.IsNullOrWhiteSpace(p.Key)).ToList();
            LoadSettings.LoadSettingsFromDatabase(st, configs);
            LoadSettings.LoadEmailSettings(st, configs);

            builder.Services.AddStorage(p => { p.Storages = st.Minio.Storages; });
            var set = systemSettings.ToDictionary(p => p.Key + "", p => p);
            if (set.TryGetValue("XApiKey", out var ett)) Setting.XApiKey = ett.Value.Cast<string?>(ett.DataType) ?? "";
            if (set.TryGetValue(nameof(st.AccountDeletedAfter), out ett)) st.AccountDeletedAfter = ett.Value.Cast<uint?>(ett.DataType) ?? 0;
            if (set.TryGetValue(nameof(st.AccountCreatedAfter), out ett)) st.AccountCreatedAfter = ett.Value.Cast<uint?>(ett.DataType) ?? 0;

            var dic = systemSettings.ToDictionary(p => p.Key + "", p => p.Value + "");
            st.LoadApiUrl(dic, st.IsLocal, !string.IsNullOrWhiteSpace(st.Protocols));
            st.LoadRpcUrl(dic, st.IsLocal);
        }
        #endregion

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

        // Service
        builder.Services.AddScoped<IDeleteAccountService, DeleteAccountService>();
        builder.Services.AddScoped<IDownloadImage, DownloadImage>();
        builder.Services.AddScoped<IEmailService, EmailService>();
        builder.Services.AddScoped<IExpiredSubscriptionService, ExpiredSubscriptionService>();
        builder.Services.AddScoped(typeof(ICountService<,>), typeof(CountService<,>));
        builder.Services.AddSingleton<IEmailSender, SmtpSender>();
        builder.Services.AddScoped<ISmsService, SmsService>();
        builder.Services.AddScoped<IPublishChapterSentService, PublishChapterSentService>();
        builder.Services.AddScoped<IExclusiveUnlockService, ExclusiveUnlockService>();
        #endregion

        #region -- Setup token --
        builder.Services.AddBearerAuthentication(st.Jwt);
        builder.Services.AddResponseCaching();

        // Cookie name
        builder.Services.ConfigureApplicationCookie(p => { p.Cookie.Name = _prefix; });
        #endregion

        builder.Services.AddControllers();

        // AddHostedService
        builder.Services.AddHostedService<HostedDeleteAccount>();
        //builder.Services.AddHostedService<HostedDownloadImage>(); not in use for now
        builder.Services.AddHostedService<HostedExpiredSubscription>();
        builder.Services.AddHostedService<HostedEmail>();
        builder.Services.AddHostedService<HostedExclusiveUnlock>();
        builder.Services.AddHostedService<HostedPublishChapterSent>();
        builder.Services.AddHostedService<HostedSmartCountComment>();
        builder.Services.AddHostedService<HostedSmartCountReact>();
        builder.Services.AddHostedService<HostedSmartLoopkup>();
        builder.Services.AddHostedService<HostedSms>();
        builder.Services.AddHostedService<HostedViewHistory>();
        builder.Services.AddQuartz(q =>
        {
            // Just use the name of your job that you created in the Jobs folder.
            var jobKey = new JobKey("RemindExpiredSubscriptionJob");
            q.AddJob<RemindExpiredSubscriptionJob>(opts => opts.WithIdentity(jobKey));

            q.AddTrigger(opts => opts
                .ForJob(jobKey)
                .WithIdentity("RemindExpiredSubscriptionJob-trigger")
                //This Cron interval can be described as "run every minute" (when second is zero)
                .WithCronSchedule("0 0 0 * * ?")
            );
        });
        builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(p => { p.EnableAnnotations(); });

        builder.Services.ConfigureMaxRequestSizes();

        var app = builder.Build();

        Setting.DevelopmentMode = st.DevMode;
        st.LogInfor();

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

        if (!st.IsLocal)
        {
            app.UseHttpsRedirection();
        }
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
    private static string _prefix = "Job";

    #endregion
}
