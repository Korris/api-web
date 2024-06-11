using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace Mcsg.Function.Job;

using Common.Core.Extensions;
using Extensions;
using Interfaces;
using Lib.Common.Mail;
using Lib.Common.Models;
using Lib.Data;
using Lib.Data.Wallet;
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
        config.LoadSettings(st.Email, "Notification:Email");
        #endregion

        // Update connection string
        cs = st.SetDbParams(cs);

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

        // Notification sent via email (using SMTP)
        builder.Services.AddNotification(p =>
        {
            p.Host = st.Email.Host;
            p.Port = st.Email.Port;
            p.UserName = st.Email.UserName;
            p.Password = st.Email.Password;
            p.SenderEmail = st.Email.SenderEmail;
            p.SenderName = st.Email.SenderName;
        });
        builder.Services.Configure<SmtpSettings>(p =>
        {
            p.SmtpHost = st.Email.Host;
            p.SmtpPort = st.Email.Port;
            p.SmtpUser = st.Email.UserName;
            p.SmtpPass = st.Email.Password;
            p.SmtpFrom = st.Email.SenderEmail;
            p.SmtpDisplayFrom = st.Email.SenderName;
        });

        // DbContext
        builder.Services.AddDataLibrary(cs);
        builder.Services.AddWalletDbContext(builder.Configuration);

        // Service
        builder.Services.AddScoped<IEmailService, EmailService>();
        builder.Services.AddScoped(typeof(ICountService<,>), typeof(CountService<,>));
        builder.Services.AddSingleton<IEmailSender, SmtpSender>();
        builder.Services.AddScoped<ISyncDataService, SyncDataService>();
        builder.Services.AddScoped<ISmsService, SmsService>();
        builder.Services.AddScoped<IExclusiveUnlockService, ExclusiveUnlockService>();
        builder.Services.AddScoped<IPaymentService, PaymentService>();
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
        builder.Services.AddHostedService<EmailFunction>();
        builder.Services.AddHostedService<ExclusiveUnlockFunction>();

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
    private static string _prefix = "Job";

    #endregion
}
