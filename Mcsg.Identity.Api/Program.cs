using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Reflection;

namespace Mcsg.Identity.Api;

using Checkers;
using Common.Core;
using Common.Core.Extensions;
using Common.Core.Interfaces;
using Common.Core.Middlewares;
using Common.Domain;
using Common.Domain.Entities;
using Common.Domain.Extensions;
using Common.Extensions;
using Common.SeedWork;
using Common.SeedWork.Extensions;
using Extensions;
using Interfaces;
using Microsoft.AspNetCore.Builder;
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
        config.LoadSettingOtp(st, "OtpSetting");
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

        // Checker
        builder.Services.AddScoped<IUserNameUniquenessChecker, UserNameUniquenessChecker>();

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
            builder.Services.AddStorage(p => { p.Storages = st.Minio.Storages; });

            var set = systemSettings.ToDictionary(p => p.Key + "", p => p);
            if (set.TryGetValue("XApiKey", out var ett)) Setting.XApiKey = ett.Value.Cast<string?>(ett.DataType) ?? "";
            if (set.TryGetValue(nameof(st.AccountDeletedAfter), out ett)) st.AccountDeletedAfter = ett.Value.Cast<uint?>(ett.DataType) ?? 0;
            if (set.TryGetValue(nameof(st.AccountCreatedAfter), out ett)) st.AccountCreatedAfter = ett.Value.Cast<uint?>(ett.DataType) ?? 0;
            if (set.TryGetValue(nameof(st.UserNameChangedInRemaining), out ett)) st.UserNameChangedInRemaining = ett.Value.Cast<double?>(ett.DataType) ?? 0;
            if (set.TryGetValue(nameof(st.UserNameWaitingChangedAfter), out ett)) st.UserNameWaitingChangedAfter = ett.Value.Cast<double?>(ett.DataType) ?? 0;
            if (set.TryGetValue(nameof(st.UsernameIsReserved), out ett)) st.UsernameIsReserved = ett.Value.Cast<string?>(ett.DataType) ?? "";

            //var dic = systemSettings.ToDictionary(p => p.Key + "", p => p.Value + "");
            //st.LoadApiUrl(dic, st.IsLocal, !string.IsNullOrWhiteSpace(st.Protocols));
            //st.LoadRpcUrl(dic, st.IsLocal);
        }
        #endregion

        // MediatR
        builder.Services.AddMediatR(p =>
        {
            p.RegisterServicesFromAssembly(me.Assembly);

            p.AddDiUserAuthenticator();
            p.AddDiUserRecovery();
            p.AddDiUserReferral();
            p.AddDiUser();
            p.AddDiFeedback();
            p.AddDiRating();
            p.AddDiVerification();
        });
        #endregion

        #region -- Setup token --
        builder.Services.AddBearerAuthentication(st.Jwt);
        builder.Services.AddResponseCaching();

        // Cookie name
        builder.Services.ConfigureApplicationCookie(p => { p.Cookie.Name = _prefix; });

        // OpenIddict
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

        // Add services to the container.
        builder.Services.AddRouting(options => options.LowercaseUrls = true);
        builder.Services.AddControllers();
        builder.Services.AddGrpc();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerDocumentation();

        builder.Services.AddIdentity<LocalizeIdentityErrorDescriber>();

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<ISecurityService, SecurityService>();
        builder.Services.AddEmailSender();
        builder.Services.AddDistributionLibrary(Assembly.GetExecutingAssembly());

        builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<ITokenService, TokenService>();
        builder.Services.AddSSOService();
        builder.Services.AddScoped<IOtpService, OtpService>();

        var app = builder.Build();

        // https://stackoverflow.com/questions/69961449/net6-and-datetime-problem-cannot-write-datetime-with-kind-utc-to-postgresql-ty


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
            app.UseCors(p => p
        .WithOrigins(origins)
        .SetIsOriginAllowed(_ => true) // Cho phép tất cả origins được khai báo
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials());
            // app.UseCors(p => p.AllowAnyHeader().AllowAnyMethod().WithOrigins(origins).AllowCredentials());
        }
        #endregion

        if (!st.IsLocal)
        {
            app.UseHttpsRedirection();
        }
        app.UseMiddleware<ResponseExceptionWrapperMiddleware>();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
        app.MapHealthChecks("/health");
        app.UseResponseCaching();

        app.MapGrpcService<Protos.Services.UserService>();
        app.MapGrpcService<Protos.Services.UserAuthenticatorService>();
        app.MapGrpcService<Protos.Services.UserRecoveryService>();

        app.Run();
    }

    #endregion

    #region -- Fields --

    /// <summary>
    /// Variable prefix
    /// </summary>
    private static string _prefix = "Ide";

    #endregion
}
