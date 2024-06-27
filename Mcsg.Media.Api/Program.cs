using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace Mcsg.Media.Api;

using Common.Core.Extensions;
using Common.SeedWork.Extensions;
using Extensions;
using Interfaces;
using Lib.Data;
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
        builder.Services.AddDbContext<McsgDbContext>(p => p.UseNpgsql(csDb!, p => p.MigrationsAssembly(assembly).EnableRetryOnFailure()), ServiceLifetime.Scoped);

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

        // MediatR
        builder.Services.AddMediatR(p =>
        {
            p.RegisterServicesFromAssembly(me.Assembly);

            p.AddDiPatch();
        });
        #endregion

        #region -- Setup token --
        // JWT
        var key = Encoding.UTF8.GetBytes(st.Jwt.Signing);
        builder.Services.AddAuthentication(p =>
        {
            p.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            p.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(p =>
        {
            p.RequireHttpsMetadata = false;
            p.SaveToken = true;
            p.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero // so tokens expire exactly at token expiration time (instead of 5 minutes later)
            };
        });

        // Add policy
        builder.Services.AddAuthorization(p =>
        {
            p.AddPolicy(Policy.AppAdmin, q => q.RequireRole(Role.SuperAdmin, Role.Admin));
            p.AddPolicy(Policy.ClientAdmin, q => q.RequireRole(Role.SuperTenant, Role.Tenant));
            p.AddPolicy(Policy.Admin, q => q.RequireRole(Role.SuperAdmin, Role.Admin, Role.SuperTenant, Role.Tenant));
        });

        // Cookie name
        builder.Services.ConfigureApplicationCookie(options =>
        {
            options.Cookie.Name = _prefix;
        });
        #endregion

        #region -- Max request body --
        var bodySize = 256 * 1024 * 1024; // 256MB
        var bufferSize = 10 * 1024 * 1024; // 10MB
        var lengthLimit = 128 * 1024 * 1024; // 128MB

        builder.Services.Configure<IISServerOptions>(p =>
        {
            p.MaxRequestBodySize = bodySize;
            p.MaxRequestBodyBufferSize = bufferSize;
        });

        builder.Services.Configure<KestrelServerOptions>(p =>
        {
            p.Limits.MaxRequestBodySize = bodySize;
            p.Limits.MaxRequestBufferSize = bufferSize;
        });

        builder.Services.Configure<FormOptions>(p =>
        {
            p.MultipartBodyLengthLimit = lengthLimit;
        });
        #endregion

        builder.Services.AddControllers();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(p => { p.EnableAnnotations(); });

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
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }

    #endregion

    #region -- Fields --

    /// <summary>
    /// Variable prefix
    /// </summary>
    private static string _prefix = "Med";

    #endregion
}
