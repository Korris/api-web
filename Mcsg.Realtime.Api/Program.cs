using System.Reflection;

namespace Mcsg.Realtime.Api;

using Common.Core.Extensions;
using Hubs;
using Lib.AzureBlobStorage;
using Lib.Common;
using Lib.Common.Constants;
using Lib.Common.Models;
using Lib.Common.Web;
using Lib.Common.Web.Extensions;
using Lib.Common.Web.Extensions.DependencyInjection;
using Lib.Data;
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

        // Get assembly name
        var me = typeof(Program);
        var assembly = me.Assembly.GetName().Name;

        // Load settings from the environment
        var st = _prefix.ConvertEnvironmentVariable<Setting>(CommonPrefix);
        st.Prefix = _prefix;

        // Load connection string appsettings.json
        var config = new ConfigurationBuilder().AddConfiguration(builder.Configuration).Build();
        var cs = config.GetConnectionString(_prefix);

        // Add services to the container.

        builder.Services.Configure<JwtSetting>(builder.Configuration.GetSection("JWT"));

        // Add services to the container.
        builder.Services.AddRouting(options => options.LowercaseUrls = true);
        builder.Services.AddControllers();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerDocumentation(AuthenticationSchemes.JwtScheme);

        builder.Services.AddDataLibrary(builder.Configuration);
        builder.Services.AddAzureBlobStorage(builder.Configuration);
        builder.Services.AddIdentity();
        builder.Services.AddDistributionLibrary(Assembly.GetExecutingAssembly());

        //Add Authentication & Authorization Setup
        builder.Services.AddBearerAuthentication(builder.Configuration);
        builder.Services.AddResponseCaching();

        builder.Services.AddCommonWebLibrary(builder.Configuration);
        builder.Services.AddCommonLibrary(builder.Configuration);
        builder.Services.AddSignalR();
        builder.Services.AddCors();
        builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

        builder.Services.AddScoped<ICommentService, CommentService>();
        builder.Services.AddScoped<IReplyService, ReplyService>();
        builder.Services.AddScoped<ISmartCountService, SmartCountService>();
        builder.Services.AddScoped<IResourceCommentService, ResourceCommentService>();
        builder.Services.AddScoped<INotificationService, NotificationService>();
        builder.Services.AddScoped<IMentionService, MentionService>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        if (builder.Configuration["AllowedOrigins"] != null)
        {
            var origins = builder.Configuration["AllowedOrigins"].Split(";");
            app.UseCors(builder => builder
                        .WithOrigins(origins)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials()
                    );
        }
        app.UseHttpsRedirection();

        app.UseApiResponseAndExceptionWrapper();

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
