using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace Mcsg.Realtime.Api;

using Common.Core.Extensions;
using Common.Core.Middlewares;
using Common.SeedWork.Extensions;
using Extensions;
using Hubs;
using Interfaces;
using Lib.Common.Constants;
using Lib.Common.Extensions;
using Lib.Common.Web.Extensions;
using Lib.Common.Web.Extensions.DependencyInjection;
using Lib.Common.Web.RealTime.Services;
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

        // Storage
        builder.Services.AddStorage(p =>
        {
            p.BucketName = st.Minio.BucketName;
            p.Location = st.Minio.Location;
            p.EndPoint = st.Minio.EndPoint;
            p.PublicUrl = st.Minio.PublicUrl;
            p.AccessKey = st.Minio.AccessKey;
            p.SecrectKey = st.Minio.SecrectKey;
        });

        // DbContext
        builder.Services.AddDataLibrary(csDb);
        #endregion

        // Add services to the container.
        builder.Services.AddRouting(options => options.LowercaseUrls = true);
        builder.Services.AddControllers();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerDocumentation(AuthenticationSchemes.JwtScheme);

        builder.Services.AddIdentity();
        builder.Services.AddDistributionLibrary(Assembly.GetExecutingAssembly());

        //Add Authentication & Authorization Setup
        builder.Services.AddBearerAuthentication(st.Jwt);
        builder.Services.AddResponseCaching();

        builder.Services.AddCommonWebLibrary();
        builder.Services.AddEmailSender();
        builder.Services.AddCors();
        builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

        builder.Services.AddScoped<ISignalRService, SignalRService>();
        builder.Services.AddSignalR();

        // Comic
        builder.Services.AddScoped<IComicCommentService, ComicCommentService>();
        builder.Services.AddScoped<IComicReplyService, ComicReplyService>();

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
        app.UserSessionAuthorizationMiddleware();
        app.UseAuthorization();

        app.MapControllers();

        app.UseResponseCaching();

        app.MapHub<CommentHub>("/commentHub");
        app.MapHub<NotificationHub>("/notificationHub");

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
