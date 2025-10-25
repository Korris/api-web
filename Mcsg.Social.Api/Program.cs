using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Reflection;

namespace Mcsg.Social.Api;

using Attributes;
using Common.Core.Extensions;
using Common.Core.Middlewares;
using Common.Domain;
using Common.Domain.Entities;
using Common.Domain.Extensions;
using Common.Extensions;
using Common.Interfaces;
using Common.SeedWork.Converters;
using Common.SeedWork.Extensions;
using Extensions;
using Interfaces;
using Models;
using Services;
using Validators;
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

        // DbContext
        builder.Services.AddDataLibrary(cs);

        // Attribute
        builder.Services.AddScoped<MediaOnlyAttribute>();

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

            _mediaExtensionAllow = st.Minio.SocialMediaExtensionAllow;
            builder.Services.AddStorage(p => { p.Storages = st.Minio.Storages; });
            var set = systemSettings.ToDictionary(p => p.Key + "", p => p);
            if (set.TryGetValue("XApiKey", out var ett)) Setting.XApiKey = ett.Value.Cast<string?>(ett.DataType) ?? "";
            if (set.TryGetValue(nameof(st.PercentFeed), out ett)) st.PercentFeed = ett.Value.Cast<double?>(ett.DataType) ?? 0;
            if (set.TryGetValue(nameof(st.PercentComic), out ett)) st.PercentComic = ett.Value.Cast<double?>(ett.DataType) ?? 0;
            if (set.TryGetValue(nameof(st.PercentDocument), out ett)) st.PercentDocument = ett.Value.Cast<double?>(ett.DataType) ?? 0;
            if (set.TryGetValue(nameof(st.PercentStory), out ett)) st.PercentStory = ett.Value.Cast<double?>(ett.DataType) ?? 0;
            if (set.TryGetValue(nameof(st.NumberOfPosts), out ett)) st.NumberOfPosts = ett.Value.Cast<int?>(ett.DataType) ?? 0;

            var dic = systemSettings.ToDictionary(p => p.Key + "", p => p.Value + "");
            st.LoadApiUrl(dic, st.IsLocal, !string.IsNullOrWhiteSpace(st.Protocols));
            st.LoadRpcUrl(dic, st.IsLocal);
        }
        #endregion

        // Service
        builder.Services.AddScoped<IFileService, FileService>();
        builder.Services.AddScoped<IJobService, JobService>();
        builder.Services.AddScoped<IPostService, PostService>();
        builder.Services.AddScoped<IMetaDataService, MetaDataService>();
        builder.Services.AddScoped<ITagService, TagService>();
        builder.Services.AddScoped<ISoundService, SoundService>();
        builder.Services.AddScoped<IPostLinkService, PostLinkService>();
        builder.Services.AddScoped<ISmartLookupService, SmartLookupService>();
        builder.Services.AddDistributionLibrary(Assembly.GetExecutingAssembly());

        // MediatR
        builder.Services.AddMediatR(p =>
        {
            p.RegisterServicesFromAssembly(me.Assembly);

            p.AddDiPost();
            p.AddDiPostFavorite();
            p.AddDiReport();
            p.AddDiSubPost();
            p.AddDiTag();
        });
        #endregion

        #region -- Setup token --
        builder.Services.AddBearerAuthentication(st.Jwt);
        builder.Services.AddResponseCaching();

        // Cookie name
        builder.Services.ConfigureApplicationCookie(p => { p.Cookie.Name = _prefix; });
        #endregion

        builder.Services.Configure<FeedDisplayConfig>(builder.Configuration.GetSection("FeedDisplayConfigs"));

        // Add services to the container.
        builder.Services.AddRouting(options => options.LowercaseUrls = true);
        builder.Services.AddGrpc();
        builder.Services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new IsoDateTimeConverter());
        });

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerDocumentation();

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddEmailSender();
        builder.Services.AddFileUploadLimit(st.Minio);
        builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

        builder.Services.AddScoped<IFeedService, FeedService>();
        builder.Services.AddScoped<ILinkPreviewService, LinkPreviewService>();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<ICommentService, CommentService>();
        builder.Services.AddScoped<IChartService, ChartService>();
        builder.Services.AddScoped<IPostReactService, PostReactService>();
        builder.Services.AddScoped<ISubPostReactService, SubPostReactService>();
        builder.Services.AddScoped<IPostCommentReactService, PostCommentReactService>();
        builder.Services.AddScoped<ISubPostCommentReactService, SubPostCommentReactService>();
        builder.Services.AddScoped(typeof(IReactService<>), typeof(ReactService<>));
        builder.Services.AddScoped<ISmartCountService, SmartCountService>();
        builder.Services.AddScoped<INotificationService, NotificationService>();

        //validator
        builder.Services.AddScoped<IValidator<TagFavorite>, TagFavoriteValidator>();
        builder.Services.AddScoped<IValidator<SocialPostFavorite>, PostFavoriteValidator>();

        builder.Services.AddScoped<IFavoriteService, FavoriteService>();

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
        app.UseApiPathRewrite(MicroServices.GetValueOrDefault(_prefix).ToLower());
        app.UseRouting();
        app.UseMiddleware<ResponseExceptionWrapperMiddleware>();
        app.UseAuthentication();
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
    private static string _prefix = "Soc";

    /// <summary>
    /// Media extension allow
    /// </summary>
    public static string _mediaExtensionAllow = default!;

    #endregion
}
