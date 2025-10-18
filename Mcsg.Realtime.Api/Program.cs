using Microsoft.AspNetCore.Server.Kestrel.Core;
using Serilog;
using System.Reflection;

namespace Mcsg.Realtime.Api;

using Common.Core.Extensions;
using Common.Core.Middlewares;
using Common.Domain;
using Common.Domain.Entities;
using Common.Domain.Extensions;
using Common.Extensions;
using Common.SeedWork.Extensions;
using Extensions;
using Hubs;
using Interfaces;
using Mcsg.Common.Core;
using Mcsg.Common.Core.Interfaces;
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



        // Business
        builder.Services.AddScoped<IBusinessText, BusinessText>();

        // Firebase
        var environment = st.Environment == "local" ? "" : "." + st.Environment;
        var credentialPath = $"{AppContext.BaseDirectory}firebase{environment}.json";
        builder.Services.AddNotification(p =>
        {
            p.CredentialPath = credentialPath;
        });

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
            LoadSettings.LoadRedisSettings(st, configs);

            builder.Services.AddStorage(p => { p.Storages = st.Minio.Storages; });
            var set = systemSettings.ToDictionary(p => p.Key + "", p => p);
            if (set.TryGetValue("XApiKey", out var ett)) Setting.XApiKey = ett.Value.Cast<string?>(ett.DataType) ?? "";

            //var dic = systemSettings.ToDictionary(p => p.Key + "", p => p.Value + "");
            //st.LoadApiUrl(dic, st.IsLocal, !string.IsNullOrWhiteSpace(st.Protocols));
            //st.LoadRpcUrl(dic, st.IsLocal);
        }
        #endregion

        // RedisStore
        builder.Services.AddSingleton<IRedisStore>(p => new RedisStore(st.Redis));

        // MediatR
        builder.Services.AddMediatR(p =>
        {
            p.RegisterServicesFromAssembly(me.Assembly);

            p.AddDiAuthentication();
        });
        #endregion

        #region -- Setup token --
        builder.Services.AddBearerAuthentication(st.Jwt);
        builder.Services.AddResponseCaching();

        // Cookie name
        builder.Services.ConfigureApplicationCookie(p => { p.Cookie.Name = _prefix; });
        #endregion

        // Add services to the container.
        builder.Services.AddRouting(options => options.LowercaseUrls = true);
        builder.Services.AddControllers();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerDocumentation();

        builder.Services.AddIdentity();
        builder.Services.AddDistributionLibrary(Assembly.GetExecutingAssembly());

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddEmailSender();
        builder.Services.AddCors();
        builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

        builder.Services.AddSignalR();

        // Comic
        builder.Services.AddScoped<IComicCommentService, ComicCommentService>();
        builder.Services.AddScoped<IComicReplyService, ComicReplyService>();

        // Document
        builder.Services.AddScoped<IDocumentCommentService, DocumentCommentService>();
        builder.Services.AddScoped<IDocumentReplyService, DocumentReplyService>();

        // Social
        builder.Services.AddScoped<ISocialCommentService, SocialCommentService>();
        builder.Services.AddScoped<ISocialReplyService, SocialReplyService>();

        // Story
        builder.Services.AddScoped<IStoryCommentService, StoryCommentService>();
        builder.Services.AddScoped<IStoryReplyService, StoryReplyService>();

        builder.Services.AddScoped<ISmartCountService, SmartCountService>();
        builder.Services.AddScoped<IResourceCommentService, ResourceCommentService>();
        builder.Services.AddScoped<INotificationService, NotificationService>();
        builder.Services.AddScoped<IStoryNotificationService, StoryNotificationService>();
        builder.Services.AddScoped<IComicNotificationService, ComicNotificationService>();
        builder.Services.AddScoped<IMentionService, MentionService>();
        builder.Services.AddScoped<IFollowService, FollowService>();
        builder.Services.AddScoped<IFollowPostService, FollowPostService>();

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

        app.UseHttpsRedirection();
        app.UseMiddleware<ResponseExceptionWrapperMiddleware>();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
        app.MapHealthChecks("/health");
        app.UseResponseCaching();

        app.MapHub<CommentHub>("/commentHub");
        app.MapHub<NotificationHub>("/notificationHub");
        app.MapHub<FollowHub>("/followHub");

        app.Run();
    }

    #endregion

    #region -- Fields --

    /// <summary>
    /// Variable prefix
    /// </summary>
    private static string _prefix = "Rea";

    #endregion
}
