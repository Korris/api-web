using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Reflection;

namespace Mcsg.Api;

using Common.Core;
using Common.Core.Extensions;
using Common.Core.Interfaces;
using Common.Core.Middlewares;
using Common.Domain;
using Common.Domain.Entities;
using Common.Domain.Extensions;
using Common.Extensions;
using Common.Interfaces;
using Common.SeedWork;
using Common.SeedWork.Extensions;
using Interfaces;
using Areas.Identity.Checkers;
using Areas.Identity.Extensions;
using Areas.Identity.Interfaces;
using Areas.Identity.Services;
using Areas.Realtime.Extensions;
using Common.Core.Interfaces;
using static Common.Core.Constants.Setting;
using static Common.SeedWork.Constants.Setting;

/// <summary>
/// Unified API - Consolidated monolith entry point
/// Merges: Comic, Story, Document, Social, Identity, Realtime
/// </summary>
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        // Readiness check (/health/ready) waits for StartupWarmupHostedService; plain /health stays a liveness probe
        builder.Services.AddHealthChecks()
            .AddCheck<Services.WarmupReadinessHealthCheck>("warmup", tags: [Services.WarmupReadinessHealthCheck.Tag]);
        builder.Services.AddHostedService<Services.StartupWarmupHostedService>();
        // Re-applies system.SystemSettings every 30s so DB edits (e.g. RpcChatChat) reach every replica without a restart
        builder.Services.AddHostedService<Services.SystemSettingsRefreshHostedService>();
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        var me = typeof(Program);
        var assembly = me.Assembly.GetName().Name;

        // Load unified settings from environment
        var st = _prefix.ConvertEnvironmentVariable<Setting>(CommonPrefix);
        st.Prefix = _prefix;

        var aspEnv = builder.Environment.EnvironmentName;

        // Database connection
        var config = new ConfigurationBuilder().AddConfiguration(builder.Configuration).Build();
        var cs = config.GetConnectionString("McsgConnectionString");
        Console.WriteLine($"[DATABASE CONNECTION] {cs}");

        #region -- Load settings --
        config.LoadSettings(st, "Queue:Notification");
        config.LoadSettingOtp(st, "OtpSetting");
        // Load queue settings for content domains
        Areas.Comic.Extensions.IConfigurationRootExtension.LoadSettings(config, st, "Queue:Notification");
        Areas.Story.Extensions.IConfigurationRootExtension.LoadSettings(config, st, "Queue:Notification");
        Areas.Document.Extensions.IConfigurationRootExtension.LoadSettings(config, st, "Queue:Notification");
        Areas.Social.Extensions.IConfigurationRootExtension.LoadSettings(config, st, "Queue:Notification");
        #endregion

        // Override Environment from ASPNETCORE_ENVIRONMENT after LoadSettings
        if (st.IsLocal && !string.Equals(aspEnv, "Development", StringComparison.OrdinalIgnoreCase))
        {
            st.Environment = aspEnv;
        }

        // Logger
        builder.Host.UseSerilog();
        assembly!.StartLogger();
        builder.Services.AddSingleton(Log.Logger);
        Log.Information("[STARTUP] ASPNETCORE_ENVIRONMENT={AspEnv}, Environment={Env}, IsLocal={IsLocal}, Commit=984ac9b2", aspEnv, st.Environment, st.IsLocal);

        #region -- Load HTTP protocols --
        if (!string.IsNullOrWhiteSpace(st.Protocols))
        {
            var protocols = st.Protocols.Split(';', StringSplitOptions.RemoveEmptyEntries);
            builder.WebHost.ConfigureKestrel(p =>
            {
                foreach (var i in protocols)
                {
                    var arr = i.Split('_', StringSplitOptions.RemoveEmptyEntries);
                    if (arr.Length != 2) continue;

                    var port = Convert.ToInt32(arr[1]);
                    var protocol = nameof(HttpProtocols.Http2) == arr[0]
                        ? HttpProtocols.Http2
                        : HttpProtocols.Http1;

                    p.ListenAnyIP(port, q => q.Protocols = protocol);
                }
            });
        }
        #endregion

        #region -- Setup DI --
        builder.Services.AddSingleton<ISetting>(st!);
        builder.Services.AddSingleton<ISecurityAes>(p => new SecurityAes(st.EncryptKey));
        builder.Services.AddDataLibrary(cs);

        // Identity: Checker
        builder.Services.AddScoped<IUserNameUniquenessChecker, UserNameUniquenessChecker>();
        // Hashtag links shared by the Game and TapShow areas
        builder.Services.AddScoped<Mcsg.Api.Interfaces.IPostHashtagService, Mcsg.Api.Services.PostHashtagService>();

        #region -- Load settings from DB --
        var serviceProvider = builder.Services.BuildServiceProvider();
        using (var ss = serviceProvider.GetService<IServiceScopeFactory>()!.CreateScope())
        {
            var context = ss.ServiceProvider.GetRequiredService<IMcsgContext>();
            // Same loader SystemSettingsRefreshHostedService re-runs every 30s (and config/v1/Reload on demand)
            var systemSettings = Services.SystemSettingsLoader.LoadAsync(context).GetAwaiter().GetResult();

            var configs = context.SystemConfigs.Where(p => !string.IsNullOrWhiteSpace(p.Key)).ToList();
            LoadSettings.LoadSettingsFromDatabase(st, configs);

            // Fingerprint of the JWT signing secret (never the raw value) so it can be compared with api-chat's "[JWT SECRET]" log line.
            var signing = st.Jwt.Signing ?? "";
            var signingHash = Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(signing)))[..12];
            Log.Information("[JWT SIGNING] Length={Length}, Prefix={Prefix}, Sha256={Sha256}, Issuer={Issuer}, Audience={Audience}",
                signing.Length, signing.Length >= 4 ? signing[..4] : signing, signingHash, st.Jwt.Issuer, st.Jwt.Audience);

            // Allowed upload extensions per area, read by each area's MediaOnlyAttribute.
            // The standalone services set these in their own Program.cs; without them every upload is rejected with "Only media files are allowed."
            Areas.Comic.Constants.ComicConfig.MediaExtensionAllow = st.Minio.ComicMediaExtensionAllow;
            Areas.Story.Constants.StoryConfig.MediaExtensionAllow = st.Minio.StoryMediaExtensionAllow;
            Areas.Social.Constants.SocialConfig.MediaExtensionAllow = st.Minio.SocialMediaExtensionAllow;
            Areas.Document.Constants.DocumentConfig.MediaExtensionAllow = st.Minio.DocumentMediaExtensionAllow;
            Areas.Game.Constants.GameConfig.MediaExtensionAllow = st.Minio.ComicMediaExtensionAllow;
            Areas.TapShow.Constants.TapShowConfig.MediaExtensionAllow = st.Minio.ComicMediaExtensionAllow;

            builder.Services.AddStorage(p => { p.Storages = st.Minio.Storages; });
            builder.Services.AddNotification(p =>
            {
                p.Host = st.Email.Host;
                p.Port = st.Email.Port;
                p.UserName = st.Email.UserName;
                p.Password = st.Email.Password;
                p.SenderEmail = st.Email.SenderEmail;
                p.SenderName = st.Email.SenderName;
            });

            Services.SystemSettingsLoader.Apply(st, systemSettings);

            Log.Information("[FEED SETTINGS] NumberOfPosts={NumberOfPosts}, PercentFeed={PercentFeed}, PercentComic={PercentComic}, PercentDocument={PercentDocument}, PercentStory={PercentStory}, PercentTapShow={PercentTapShow}", st.NumberOfPosts, st.PercentFeed, st.PercentComic, st.PercentDocument, st.PercentStory, st.PercentTapShow);
            Log.Information("[RPC CONFIG] Analytic={Analytic}, Chat={Chat}, IsLocal={IsLocal}", st.Rpc.Analytic.Analytic, st.Rpc.Chat.Chat, st.IsLocal);
        }
        #endregion

        // MediatR - scan this assembly for all handlers across all areas
        builder.Services.AddMediatR(p =>
        {
            p.RegisterServicesFromAssembly(me.Assembly);

            // Identity MediatR handlers
            p.AddDiUserAuthenticator();
            p.AddDiUserRecovery();
            p.AddDiUserReferral();
            p.AddDiUser();
            p.AddDiFeedback();
            p.AddDiRating();
            p.AddDiVerification();

            // Realtime MediatR handlers
            p.AddDiRealtimeAuthentication();

            // Comic MediatR handlers
            Areas.Comic.Extensions.DiPostExtension.AddDiPost(p);
            Areas.Comic.Extensions.DiReportExtension.AddDiReport(p);
            Areas.Comic.Extensions.DiSubPostExtension.AddDiSubPost(p);

            // Story MediatR handlers
            Areas.Story.Extensions.DiPostExtension.AddDiPost(p);
            Areas.Story.Extensions.DiReportExtension.AddDiReport(p);
            Areas.Story.Extensions.DiSubPostExtension.AddDiSubPost(p);

            // Document MediatR handlers
            Areas.Document.Extensions.DiPostExtension.AddDiPost(p);
            Areas.Document.Extensions.DiReportExtension.AddDiReport(p);
            Areas.Document.Extensions.DiSubPostExtension.AddDiSubPost(p);
            Areas.Document.Extensions.DiResourceExtension.AddDiResource(p);

            // Social MediatR handlers
            Areas.Social.Extensions.DiPostExtension.AddDiPost(p);
            Areas.Social.Extensions.DiPostFavoriteExtension.AddDiPostFavorite(p);
            Areas.Social.Extensions.DiReportExtension.AddDiReport(p);
            Areas.Social.Extensions.DiSubPostExtension.AddDiSubPost(p);
            Areas.Social.Extensions.DiTagExtension.AddDiTag(p);
        });
        #endregion

        #region -- Auth & Token --
        builder.Services.AddBearerAuthentication(st.Jwt);
        builder.Services.AddResponseCaching();
        builder.Services.ConfigureApplicationCookie(p => { p.Cookie.Name = _prefix; });

        // OpenIddict (from Identity)
        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, p => { p.LoginPath = "/Account/Login"; });
        builder.Services.AddOpenIddict()
            .AddCore(p => { p.UseEntityFrameworkCore().UseDbContext<McsgContext>(); })
            .AddServer(p =>
            {
                p.AllowClientCredentialsFlow().AllowAuthorizationCodeFlow().RequireProofKeyForCodeExchange().AllowRefreshTokenFlow();
                p.SetTokenEndpointUris("/connect/token").SetAuthorizationEndpointUris("/connect/authorize").SetUserInfoEndpointUris("/connect/userinfo");
                p.AddEphemeralEncryptionKey().AddEphemeralSigningKey().DisableAccessTokenEncryption();
                p.RegisterScopes("api");
                p.UseAspNetCore().DisableTransportSecurityRequirement().EnableTokenEndpointPassthrough().EnableAuthorizationEndpointPassthrough().EnableUserInfoEndpointPassthrough();
            });
        #endregion

        // Routing, Controllers, gRPC, SignalR
        builder.Services.AddRouting(options => options.LowercaseUrls = true);
        builder.Services.AddControllers();
        builder.Services.AddGrpc();
        builder.Services.AddSignalR();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerDocumentation();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddDistributionLibrary(Assembly.GetExecutingAssembly());

        // Identity services
        builder.Services.AddIdentity<LocalizeIdentityErrorDescriber>();
        builder.Services.AddScoped<ISecurityService, SecurityService>();
        builder.Services.AddEmailSender();
        builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<ITokenService, TokenService>();
        builder.Services.AddSSOService();
        builder.Services.AddScoped<IOtpService, OtpService>();

        // Realtime services
        builder.Services.AddSingleton<IRedisStore>(p => new RedisStore(st.Redis));
        builder.Services.AddScoped<Areas.Realtime.Interfaces.IComicCommentService, Areas.Realtime.Services.ComicCommentService>();
        builder.Services.AddScoped<Areas.Realtime.Interfaces.IComicReplyService, Areas.Realtime.Services.ComicReplyService>();
        builder.Services.AddScoped<Areas.Realtime.Interfaces.IDocumentCommentService, Areas.Realtime.Services.DocumentCommentService>();
        builder.Services.AddScoped<Areas.Realtime.Interfaces.IDocumentReplyService, Areas.Realtime.Services.DocumentReplyService>();
        builder.Services.AddScoped<Areas.Realtime.Interfaces.ISocialCommentService, Areas.Realtime.Services.SocialCommentService>();
        builder.Services.AddScoped<Areas.Realtime.Interfaces.ISocialReplyService, Areas.Realtime.Services.SocialReplyService>();
        builder.Services.AddScoped<Areas.Realtime.Interfaces.IStoryCommentService, Areas.Realtime.Services.StoryCommentService>();
        builder.Services.AddScoped<Areas.Realtime.Interfaces.IStoryReplyService, Areas.Realtime.Services.StoryReplyService>();
        builder.Services.AddScoped<Areas.Realtime.Interfaces.ITapShowCommentService, Areas.Realtime.Services.TapShowCommentService>();
        builder.Services.AddScoped<Areas.Realtime.Interfaces.ITapShowReplyService, Areas.Realtime.Services.TapShowReplyService>();
        builder.Services.AddScoped<Areas.Realtime.Interfaces.ISmartCountService, Areas.Realtime.Services.SmartCountService>();
        builder.Services.AddScoped<Areas.Realtime.Interfaces.IResourceCommentService, Areas.Realtime.Services.ResourceCommentService>();
        builder.Services.AddScoped<Areas.Realtime.Interfaces.INotificationService, Areas.Realtime.Services.NotificationService>();
        builder.Services.AddScoped<Areas.Realtime.Interfaces.IStoryNotificationService, Areas.Realtime.Services.StoryNotificationService>();
        builder.Services.AddScoped<Areas.Realtime.Interfaces.IComicNotificationService, Areas.Realtime.Services.ComicNotificationService>();
        builder.Services.AddScoped<Areas.Realtime.Interfaces.IMentionService, Areas.Realtime.Services.MentionService>();
        builder.Services.AddScoped<Areas.Realtime.Interfaces.IFollowService, Areas.Realtime.Services.FollowService>();
        builder.Services.AddScoped<Areas.Realtime.Interfaces.IFollowPostService, Areas.Realtime.Services.FollowPostService>();

        // Shared services (used across Comic, Story, Document, Social)
        builder.Services.AddScoped<IBusinessText, BusinessText>();
        Areas.Comic.Extensions.ServiceCollectionExtensions.AddFileUploadLimit(builder.Services, st.Minio);

        // Game services
        Areas.Game.Extensions.ServiceCollectionExtensions.AddGameServices(builder.Services);

        // TapShow services
        Areas.TapShow.Extensions.ServiceCollectionExtensions.AddTapShowServices(builder.Services);

        // Comic services
        builder.Services.AddScoped<Areas.Comic.Attributes.MediaOnlyAttribute>();
        builder.Services.AddScoped<Areas.Comic.Interfaces.IFileService, Areas.Comic.Services.FileService>();
        builder.Services.AddScoped<Areas.Comic.Interfaces.IJobService, Areas.Comic.Services.JobService>();
        builder.Services.AddScoped<Areas.Comic.Interfaces.IPostService, Areas.Comic.Services.PostService>();
        builder.Services.AddScoped<Areas.Comic.Interfaces.IMetaDataService, Areas.Comic.Services.MetaDataService>();
        builder.Services.AddScoped<Areas.Comic.Interfaces.ITagService, Areas.Comic.Services.TagService>();
        builder.Services.AddScoped<Areas.Comic.Interfaces.ISoundService, Areas.Comic.Services.SoundService>();
        builder.Services.AddScoped<Areas.Comic.Interfaces.IPostLinkService, Areas.Comic.Services.PostLinkService>();
        builder.Services.AddScoped<Areas.Comic.Interfaces.ISmartLookupService, Areas.Comic.Services.SmartLookupService>();
        builder.Services.AddScoped<Areas.Comic.Interfaces.IFeedService, Areas.Comic.Services.FeedService>();
        builder.Services.AddScoped<Areas.Comic.Interfaces.ILinkPreviewService, Areas.Comic.Services.LinkPreviewService>();
        builder.Services.AddScoped<Areas.Comic.Interfaces.ICommentService, Areas.Comic.Services.CommentService>();
        builder.Services.AddScoped<Areas.Comic.Interfaces.IPostReactService, Areas.Comic.Services.PostReactService>();
        builder.Services.AddScoped<Areas.Comic.Interfaces.ISubPostReactService, Areas.Comic.Services.SubPostReactService>();
        builder.Services.AddScoped<Areas.Comic.Interfaces.IPostCommentReactService, Areas.Comic.Services.PostCommentReactService>();
        builder.Services.AddScoped<Areas.Comic.Interfaces.ISubPostCommentReactService, Areas.Comic.Services.SubPostCommentReactService>();
        builder.Services.AddScoped(typeof(Areas.Comic.Interfaces.IReactService<>), typeof(Areas.Comic.Services.ReactService<>));
        builder.Services.AddScoped<Areas.Comic.Interfaces.IComicService, Areas.Comic.Services.ComicService>();
        builder.Services.AddScoped<Areas.Comic.Interfaces.ISmartCountService, Areas.Comic.Services.SmartCountService>();
        builder.Services.AddScoped<IValidator<Common.Domain.Entities.TagFavorite>, Areas.Comic.Validators.TagFavoriteValidator>();
        builder.Services.AddScoped<IValidator<Common.Domain.Entities.ComicPostFavorite>, Areas.Comic.Validators.PostFavoriteValidator>();
        builder.Services.AddScoped<Areas.Comic.Interfaces.IFavoriteService, Areas.Comic.Services.FavoriteService>();
        builder.Services.Configure<Areas.Comic.Models.FeedDisplayConfig>(builder.Configuration.GetSection("FeedDisplayConfigs"));

        // Story services
        builder.Services.AddScoped<Areas.Story.Attributes.MediaOnlyAttribute>();
        builder.Services.AddScoped<Areas.Story.Interfaces.IFileService, Areas.Story.Services.FileService>();
        builder.Services.AddScoped<Areas.Story.Interfaces.IJobService, Areas.Story.Services.JobService>();
        builder.Services.AddScoped<Areas.Story.Interfaces.IPostService, Areas.Story.Services.PostService>();
        builder.Services.AddScoped<Areas.Story.Interfaces.IMetaDataService, Areas.Story.Services.MetaDataService>();
        builder.Services.AddScoped<Areas.Story.Interfaces.ITagService, Areas.Story.Services.TagService>();
        builder.Services.AddScoped<Areas.Story.Interfaces.ISoundService, Areas.Story.Services.SoundService>();
        builder.Services.AddScoped<Areas.Story.Interfaces.IPostLinkService, Areas.Story.Services.PostLinkService>();
        builder.Services.AddScoped<Areas.Story.Interfaces.ISmartLookupService, Areas.Story.Services.SmartLookupService>();
        builder.Services.AddScoped<Areas.Story.Interfaces.IFeedService, Areas.Story.Services.FeedService>();
        builder.Services.AddScoped<Areas.Story.Interfaces.ILinkPreviewService, Areas.Story.Services.LinkPreviewService>();
        builder.Services.AddScoped<Areas.Story.Interfaces.ICommentService, Areas.Story.Services.CommentService>();
        builder.Services.AddScoped<Areas.Story.Interfaces.IPostReactService, Areas.Story.Services.PostReactService>();
        builder.Services.AddScoped<Areas.Story.Interfaces.ISubPostReactService, Areas.Story.Services.SubPostReactService>();
        builder.Services.AddScoped<Areas.Story.Interfaces.IPostCommentReactService, Areas.Story.Services.PostCommentReactService>();
        builder.Services.AddScoped<Areas.Story.Interfaces.ISubPostCommentReactService, Areas.Story.Services.SubPostCommentReactService>();
        builder.Services.AddScoped(typeof(Areas.Story.Interfaces.IReactService<>), typeof(Areas.Story.Services.ReactService<>));
        builder.Services.AddScoped<Areas.Story.Interfaces.IStoryService, Areas.Story.Services.StoryService>();
        builder.Services.AddScoped<Areas.Story.Interfaces.ISmartCountService, Areas.Story.Services.SmartCountService>();
        builder.Services.AddScoped<IValidator<Common.Domain.Entities.StoryPostFavorite>, Areas.Story.Validators.PostFavoriteValidator>();
        builder.Services.AddScoped<Areas.Story.Interfaces.IFavoriteService, Areas.Story.Services.FavoriteService>();
        builder.Services.Configure<Areas.Story.Models.FeedDisplayConfig>(builder.Configuration.GetSection("FeedDisplayConfigs"));

        // Document services
        builder.Services.AddScoped<Areas.Document.Attributes.MediaOnlyAttribute>();
        builder.Services.AddScoped<Areas.Document.Interfaces.IFileService, Areas.Document.Services.FileService>();
        builder.Services.AddScoped<Areas.Document.Interfaces.IJobService, Areas.Document.Services.JobService>();
        builder.Services.AddScoped<Areas.Document.Interfaces.IPostService, Areas.Document.Services.PostService>();
        builder.Services.AddScoped<Areas.Document.Interfaces.IMetaDataService, Areas.Document.Services.MetaDataService>();
        builder.Services.AddScoped<Areas.Document.Interfaces.ITagService, Areas.Document.Services.TagService>();
        builder.Services.AddScoped<Areas.Document.Interfaces.ISoundService, Areas.Document.Services.SoundService>();
        builder.Services.AddScoped<Areas.Document.Interfaces.IPostLinkService, Areas.Document.Services.PostLinkService>();
        builder.Services.AddScoped<Areas.Document.Interfaces.ISmartLookupService, Areas.Document.Services.SmartLookupService>();
        builder.Services.AddScoped<Areas.Document.Interfaces.IFeedService, Areas.Document.Services.FeedService>();
        builder.Services.AddScoped<Areas.Document.Interfaces.ILinkPreviewService, Areas.Document.Services.LinkPreviewService>();
        builder.Services.AddScoped<Areas.Document.Interfaces.ICommentService, Areas.Document.Services.CommentService>();
        builder.Services.AddScoped<Areas.Document.Interfaces.IPostReactService, Areas.Document.Services.PostReactService>();
        builder.Services.AddScoped<Areas.Document.Interfaces.ISubPostReactService, Areas.Document.Services.SubPostReactService>();
        builder.Services.AddScoped<Areas.Document.Interfaces.IPostCommentReactService, Areas.Document.Services.PostCommentReactService>();
        builder.Services.AddScoped<Areas.Document.Interfaces.ISubPostCommentReactService, Areas.Document.Services.SubPostCommentReactService>();
        builder.Services.AddScoped(typeof(Areas.Document.Interfaces.IReactService<>), typeof(Areas.Document.Services.ReactService<>));
        builder.Services.AddScoped<Areas.Document.Interfaces.IDocumentService, Areas.Document.Services.DocumentService>();
        builder.Services.AddScoped<Areas.Document.Interfaces.ISmartCountService, Areas.Document.Services.SmartCountService>();
        builder.Services.AddScoped<IValidator<Common.Domain.Entities.DocumentPostFavorite>, Areas.Document.Validators.PostFavoriteValidator>();
        builder.Services.AddScoped<Areas.Document.Interfaces.IFavoriteService, Areas.Document.Services.FavoriteService>();
        builder.Services.Configure<Areas.Document.Models.FeedDisplayConfig>(builder.Configuration.GetSection("FeedDisplayConfigs"));

        // Social services
        builder.Services.AddScoped<Areas.Social.Attributes.MediaOnlyAttribute>();
        builder.Services.AddScoped<Areas.Social.Interfaces.IFileService, Areas.Social.Services.FileService>();
        builder.Services.AddScoped<Areas.Social.Interfaces.IJobService, Areas.Social.Services.JobService>();
        builder.Services.AddScoped<Areas.Social.Interfaces.IPostService, Areas.Social.Services.PostService>();
        builder.Services.AddScoped<Areas.Social.Interfaces.IMetaDataService, Areas.Social.Services.MetaDataService>();
        builder.Services.AddScoped<Areas.Social.Interfaces.ITagService, Areas.Social.Services.TagService>();
        builder.Services.AddScoped<Areas.Social.Interfaces.ISoundService, Areas.Social.Services.SoundService>();
        builder.Services.AddScoped<Areas.Social.Interfaces.IPostLinkService, Areas.Social.Services.PostLinkService>();
        builder.Services.AddScoped<Areas.Social.Interfaces.ISmartLookupService, Areas.Social.Services.SmartLookupService>();
        builder.Services.AddScoped<Areas.Social.Interfaces.IFeedService, Areas.Social.Services.FeedService>();
        builder.Services.AddScoped<Areas.Social.Interfaces.IHomeFeedAggregationService, Areas.Social.Services.HomeFeedAggregationService>(); // home page: ranked ids + hydrated boxes in one call
        builder.Services.AddScoped<Areas.Social.Interfaces.ILinkPreviewService, Areas.Social.Services.LinkPreviewService>();
        builder.Services.AddScoped<Areas.Social.Interfaces.IUserService, Areas.Social.Services.UserService>(); // Social-specific IUserService (not Identity)
        builder.Services.AddScoped<Areas.Social.Interfaces.ICommentService, Areas.Social.Services.CommentService>();
        builder.Services.AddScoped<Areas.Social.Interfaces.IChartService, Areas.Social.Services.ChartService>();
        builder.Services.AddScoped<Areas.Social.Interfaces.IPostReactService, Areas.Social.Services.PostReactService>();
        builder.Services.AddScoped<Areas.Social.Interfaces.ISubPostReactService, Areas.Social.Services.SubPostReactService>();
        builder.Services.AddScoped<Areas.Social.Interfaces.IPostCommentReactService, Areas.Social.Services.PostCommentReactService>();
        builder.Services.AddScoped<Areas.Social.Interfaces.ISubPostCommentReactService, Areas.Social.Services.SubPostCommentReactService>();
        builder.Services.AddScoped(typeof(Areas.Social.Interfaces.IReactService<>), typeof(Areas.Social.Services.ReactService<>));
        builder.Services.AddScoped<Areas.Social.Interfaces.ISmartCountService, Areas.Social.Services.SmartCountService>();
        builder.Services.AddScoped<Areas.Social.Interfaces.INotificationService, Areas.Social.Services.NotificationService>();
        builder.Services.AddScoped<IValidator<Common.Domain.Entities.SocialPostFavorite>, Areas.Social.Validators.PostFavoriteValidator>();
        builder.Services.AddScoped<Areas.Social.Interfaces.IFavoriteService, Areas.Social.Services.FavoriteService>();
        builder.Services.Configure<Areas.Social.Models.FeedDisplayConfig>(builder.Configuration.GetSection("FeedDisplayConfigs"));

        builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

        var app = builder.Build();

        Setting.DevelopmentMode = st.DevMode;
        st.LogInfor();

        #region -- Swagger and CORS --
        if (app.Environment.IsDevelopment() || st.SwaggerEnabled)
        {
            if (st.Environment == "local")
            {
                app.UseSwagger();
            }
            else
            {
                app.UseSwagger(p => { p.RouteTemplate = "swagger/{documentName}/swagger.json"; });
            }
            app.UseSwaggerUI();
        }
        else
        {
            app.UseDeveloperExceptionPage();
        }

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

        // First in the pipeline so its timestamp is "headers reached the pod"
        app.UseMiddleware<Middlewares.RequestTimingMiddleware>();

        // No more UseApiPathRewrite - controllers have full route prefixes now
        app.UseRouting();
        app.UseMiddleware<ResponseExceptionWrapperMiddleware>();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
        // Liveness: everything except the warm-up gate, so a cold pod is not restarted. Readiness: the warm-up gate only.
        app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
        {
            Predicate = c => !c.Tags.Contains(Services.WarmupReadinessHealthCheck.Tag)
        });
        app.MapHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
        {
            Predicate = c => c.Tags.Contains(Services.WarmupReadinessHealthCheck.Tag)
        });
        app.UseResponseCaching();

        // gRPC services (Identity)
        app.MapGrpcService<Areas.Identity.Protos.Services.UserService>();
        app.MapGrpcService<Areas.Identity.Protos.Services.UserAuthenticatorService>();
        app.MapGrpcService<Areas.Identity.Protos.Services.UserRecoveryService>();

        // SignalR hubs (Realtime). Full prefix like the controllers: the old Realtime service had it stripped by UseApiPathRewrite,
        // clients connect to /api/realtime/{hub}
        app.MapHub<Areas.Realtime.Hubs.CommentHub>("/api/realtime/commentHub");
        app.MapHub<Areas.Realtime.Hubs.NotificationHub>("/api/realtime/notificationHub");
        app.MapHub<Areas.Realtime.Hubs.FollowHub>("/api/realtime/followHub");

        app.Run();
    }

    /// <summary>
    /// Unified prefix for the consolidated API
    /// </summary>
    private static string _prefix = "Api";
}
