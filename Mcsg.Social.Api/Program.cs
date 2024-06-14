using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace Mcsg.Social.Api;

using Attributes;
using Common.Core.Extensions;
using Common.SeedWork.Extensions;
using Extensions;
using Helper;
using Interfaces;
using Lib.AzureBlobStorage;
using Lib.Common;
using Lib.Common.Constants;
using Lib.Common.Interfaces;
using Lib.Common.Models;
using Lib.Common.Web;
using Lib.Common.Web.Extensions;
using Lib.Common.Web.Extensions.DependencyInjection;
using Lib.Data;
using Lib.Data.Analytic;
using Lib.Data.Domain.Entities;
using Lib.Data.Wallet;
using Models;
using Services;
using Services.Interfaces;
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
        assembly!.StartLogger(st);

        #region -- Load HTTP protocols --
        if (!st.IsLocal && !string.IsNullOrWhiteSpace(st.Protocols))
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

        // DbContext
        builder.Services.AddDataLibrary(csDb);
        builder.Services.AddWalletDbContext(builder.Configuration);
        builder.Services.AddAnalyticDbContext(builder.Configuration);
        #endregion

        builder.Services.Configure<JwtSetting>(builder.Configuration.GetSection("JWT"));
        builder.Services.Configure<FileSetting>(builder.Configuration.GetSection("FileSettings"));
        builder.Services.Configure<RealTimeServiceSetting>(builder.Configuration.GetSection("RealTimeServiceSettings"));
        builder.Services.Configure<FeedDisplayConfig>(builder.Configuration.GetSection("FeedDisplayConfigs"));

        AppSettingsProvider.Configuration = builder.Configuration;

        // Add services to the container.
        builder.Services.AddRouting(options => options.LowercaseUrls = true);
        builder.Services.AddControllers();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerDocumentation(AuthenticationSchemes.JwtScheme);

        //Add Authentication & Authorization Setup
        builder.Services.AddBearerAuthentication(builder.Configuration);
        builder.Services.AddResponseCaching();

        builder.Services.AddCommonWebLibrary(builder.Configuration);
        builder.Services.AddCommonLibrary(builder.Configuration);
        builder.Services.AddAzureBlobStorage(builder.Configuration);
        builder.Services.AddFileUploadLimit(builder.Configuration);
        builder.Services.AddDistributionLibrary(Assembly.GetExecutingAssembly());
        builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

        builder.Services.AddScoped<IFeedService, FeedService>();
        builder.Services.AddScoped<ILinkPreviewService, LinkPreviewService>();
        builder.Services.AddScoped<ITagService, TagService>();
        builder.Services.AddScoped<IFileService, FileService>();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<IMetaDataService, MetaDataService>();
        builder.Services.AddScoped<MediaOnlyAttribute>();
        builder.Services.AddScoped<IJobService, JobService>();
        builder.Services.AddScoped<ICommentService, CommentService>();
        builder.Services.AddScoped<IPostReactService, PostReactService>();
        builder.Services.AddScoped<ISubPostReactService, SubPostReactService>();
        builder.Services.AddScoped<IPostCommentReactService, PostCommentReactService>();
        builder.Services.AddScoped<ISubPostCommentReactService, SubPostCommentReactService>();
        builder.Services.AddScoped(typeof(IReactService<>), typeof(ReactService<>));
        builder.Services.AddScoped<IPostService, PostService>();
        builder.Services.AddScoped<IComicService, ComicService>();
        builder.Services.AddScoped<IStoryService, StoryService>();
        builder.Services.AddScoped<ISmartLookupService, SmartLookupService>();
        builder.Services.AddScoped<ISmartCountService, SmartCountService>();
        builder.Services.AddScoped<INotificationService, NotificationService>();
        builder.Services.AddScoped<ISoundService, SoundService>();
        builder.Services.AddScoped<IViewHistoryService, ViewHistoryService>();
        builder.Services.AddScoped<IEarningService, EarningService>();
        builder.Services.AddScoped<IUserViewService, UserViewService>();
        builder.Services.AddScoped<IAffiliateService, AffiliateService>();
        builder.Services.AddScoped<IWalletService, WalletService>();
        builder.Services.AddScoped<IPostLinkService, PostLinkService>();

        //validator
        builder.Services.AddScoped<IValidator<TagFavorite>, TagFavoriteValidator>();
        builder.Services.AddScoped<IValidator<PostFavorite>, PostFavoriteValidator>();
        builder.Services.AddScoped<IValidator<PostReport>, PostReportValidator>();

        builder.Services.AddScoped<IFavoriteService, FavoriteService>();

        builder.Services.Configure<IISServerOptions>(options =>
        {
            options.MaxRequestBodySize = int.MaxValue;
            options.MaxRequestBodyBufferSize = 10 * 1024 * 1024;
        });

        builder.Services.Configure<KestrelServerOptions>(options =>
        {
            options.Limits.MaxRequestBodySize = int.MaxValue;
            options.Limits.MaxRequestBufferSize = 10 * 1024 * 1024;
        });

        builder.Services.Configure<FormOptions>(options =>
        {
            options.MultipartBodyLengthLimit = int.MaxValue;
        });

        var app = builder.Build();

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
                        q.Servers = [new OpenApiServer { Url = $"{st.Domain}/api/{MicroServices.GetValueOrDefault(_prefix)}" }];
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

        app.UseApiResponseAndExceptionWrapper();

        app.UseAuthentication();
        app.UserSessionAuthorizationMiddleware();
        app.UseAuthorization();
        app.MapControllers();
        app.UseResponseCaching();
        app.EnableNpgsqlLegacyTime();

        app.Run();
    }

    #endregion

    #region -- Fields --

    /// <summary>
    /// Variable prefix
    /// </summary>
    private static string _prefix = "Soc";

    #endregion
}
