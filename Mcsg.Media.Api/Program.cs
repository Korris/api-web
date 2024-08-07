using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

namespace Mcsg.Media.Api;

using Checkers;
using Common.Core.Extensions;
using Common.Domain;
using Common.SeedWork;
using Common.SeedWork.Extensions;
using Extensions;
using Interfaces;
using Lib.Data.Interfaces;
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

        // SecurityAes
        builder.Services.AddSingleton<ISecurityAes>(p => new SecurityAes(st.EncryptKey));

        // DbContext
        builder.Services.AddDbContext<McsgContext>(p => p.UseNpgsql(csDb!, p => p.MigrationsAssembly(assembly).EnableRetryOnFailure()), ServiceLifetime.Scoped);
        builder.Services.AddScoped<IMcsgContext>(p => p.GetService<McsgContext>()!);

        // Checker
        builder.Services.AddScoped<IUserNameUniquenessChecker, UserNameUniquenessChecker>();

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
        builder.Services.AddBearerAuthentication(st.Jwt);

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
