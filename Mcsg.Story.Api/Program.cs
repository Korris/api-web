using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Reflection;

namespace Mcsg.Story.Api;

using Attributes;
using Common.Core;
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

        _mediaExtensionAllow = st.Minio.StoryMediaExtensionAllow;

        #region -- Setup DI --
        // Setting
        builder.Services.AddSingleton<ISetting>(st!);

        // Business
        builder.Services.AddScoped<IBusinessText, BusinessText>();

        // Storage
        st.LoadStorages();
        builder.Services.AddStorage(p => { p.Storages = st.Minio.Storages; });

        // DbContext
        builder.Services.AddDataLibrary(csDb);

        // Attribute
        builder.Services.AddScoped<MediaOnlyAttribute>();

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

        #region -- Google Sheets API --
        // Add configuration for Google Sheets Service into DI
        builder.Services.AddSingleton(s =>
        {
            // Load service account credentials JSON file
            var basePath = AppDomain.CurrentDomain.BaseDirectory;
            var filePath = Path.Combine(basePath, "serviceaccounts.json");

            GoogleCredential credential;
            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream)
                    .CreateScoped(SheetsService.Scope.Spreadsheets);
            }

            // Create Google Sheets API service
            var sheetsService = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Google Sheets API with Service Account"
            });

            return new GoogleSheet(sheetsService);
        });
        #endregion

        // MediatR
        builder.Services.AddMediatR(p =>
        {
            p.RegisterServicesFromAssembly(me.Assembly);

            p.AddDiPost();
            p.AddDiReport();
            p.AddDiSubPost();
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
        builder.Services.AddScoped<ICommentService, CommentService>();
        builder.Services.AddScoped<IPostReactService, PostReactService>();
        builder.Services.AddScoped<ISubPostReactService, SubPostReactService>();
        builder.Services.AddScoped<IPostCommentReactService, PostCommentReactService>();
        builder.Services.AddScoped<ISubPostCommentReactService, SubPostCommentReactService>();
        builder.Services.AddScoped(typeof(IReactService<>), typeof(ReactService<>));
        builder.Services.AddScoped<IStoryService, StoryService>();
        builder.Services.AddScoped<ISmartCountService, SmartCountService>();

        //validator
        builder.Services.AddScoped<IValidator<TagFavorite>, TagFavoriteValidator>();
        builder.Services.AddScoped<IValidator<StoryPostFavorite>, PostFavoriteValidator>();

        builder.Services.AddScoped<IFavoriteService, FavoriteService>();

        builder.Services.ConfigureMaxRequestSizes();

        var app = builder.Build();

        // https://stackoverflow.com/questions/69961449/net6-and-datetime-problem-cannot-write-datetime-with-kind-utc-to-postgresql-ty
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        #region -- Load settings --
        using (var ss = app.Services.GetService<IServiceScopeFactory>()!.CreateScope())
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

            var set = systemSettings.ToDictionary(p => p.Key + "", p => p);
            if (set.TryGetValue("XApiKey", out var ett)) Setting.XApiKey = ett.Value.Cast<string?>(ett.DataType) ?? "";

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
    private static string _prefix = "Sto";

    /// <summary>
    /// Media extension allow
    /// </summary>
    public static string _mediaExtensionAllow = default!;

    #endregion
}
