using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;

namespace Mcsg.OpenId.Mvc;

using Common.Core.Extensions;
using Common.Domain;
using Common.Domain.Extensions;
using Common.SeedWork.Extensions;
using Helpers;
using Interfaces;
using Lib.Common.Constants;
using Lib.Common.Web.Extensions;
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
                    var protocol = nameof(HttpProtocols.Http2) == arr[0] ? HttpProtocols.Http2 : HttpProtocols.Http1;

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

        // Storage
        st.LoadStorages();
        builder.Services.AddStorage(p => { p.Storages = st.Minio.Storages; });

        // MediatR
        builder.Services.AddMediatR(p =>
        {
            p.RegisterServicesFromAssembly(me.Assembly);
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
                p.SetTokenEndpointUris("/connect/token").SetAuthorizationEndpointUris("/connect/authorize").SetUserinfoEndpointUris("/connect/userinfo");
                p.AddEphemeralEncryptionKey().AddEphemeralSigningKey().DisableAccessTokenEncryption();
                p.RegisterScopes("api");
                p.UseAspNetCore().DisableTransportSecurityRequirement().EnableTokenEndpointPassthrough().EnableAuthorizationEndpointPassthrough().EnableUserinfoEndpointPassthrough();
            });
        #endregion

        // Add services to the container.
        builder.Services.AddControllersWithViews();

        builder.Services.AddSwaggerDocumentation(AuthenticationSchemes.JwtScheme);

        builder.Services.AddIdentity<LocalizeIdentityErrorDescriber>();

        builder.Services.AddScoped<ISessionService, SessionService>();

        var app = builder.Build();

        // https://stackoverflow.com/questions/69961449/net6-and-datetime-problem-cannot-write-datetime-with-kind-utc-to-postgresql-ty
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        #region -- Load settings --
        using (var ss = app.Services.GetService<IServiceScopeFactory>()!.CreateScope())
        {
            var context = ss.ServiceProvider.GetRequiredService<IMcsgContext>();
            var dic = context.SystemSettings.Where(p => !string.IsNullOrWhiteSpace(p.Key)).ToDictionary(p => p.Key + "", p => p.Value + "");

            st.LoadApiUrl(dic, st.IsLocal, !string.IsNullOrWhiteSpace(st.Protocols));
            st.LoadRpcUrl(dic, st.IsLocal);
        }
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
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }

    #endregion

    #region -- Fields --

    /// <summary>
    /// Variable prefix
    /// </summary>
    private static string _prefix = "Ope";

    #endregion
}
