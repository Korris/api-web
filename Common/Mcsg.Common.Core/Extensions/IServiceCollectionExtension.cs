#region Information
/*
 * Author       : Toan Nguyen Van
 * Email        : nvt87x@gmail.com
 * Phone        : +84 345 515 010
 * ------------------------------- *
 * Create       : 2024-Jan-21 08:37
 * Update       : 2024-Jan-21 08:37
 * Checklist    : 1.0
 * Status       : New
 */
#endregion

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text;

namespace Mcsg.Common.Core.Extensions;

using Distributor;
using Interfaces;
using Notifications;
using SeedWork.Dtos;
using Storages;
using static SeedWork.Constants.Setting;
using static SeedWork.Dtos.ConnectionDto;
using static SeedWork.Dtos.StorageDto;

/// <summary>
/// IServiceCollection extension for using [this IServiceCollection] only
/// </summary>
public static class IServiceCollectionExtension
{
    #region -- Methods --

    /// <summary>
    /// Add notification
    /// </summary>
    /// <param name="service">Service</param>
    /// <param name="action">Configure action</param>
    /// <returns>Return the result</returns>
    public static IServiceCollection AddNotification(this IServiceCollection service, Action<NotificationDto> action)
    {
        service.Configure(action);
        service.AddSingleton<INotificationClient, NotificationClient>();

        return service;
    }

    /// <summary>
    /// Add storage
    /// </summary>
    /// <param name="service">Service</param>
    /// <param name="action">Configure action</param>
    /// <returns>Return the result</returns>
    public static IServiceCollection AddStorage(this IServiceCollection service, Action<MinioDto> action)
    {
        service.Configure(action);
        service.AddSingleton<IStorageClient, StorageClient>();

        return service;
    }

    /// <summary>
    /// Add bearer authentication
    /// </summary>
    /// <param name="service">Service</param>
    /// <param name="jwt">JWT DTO</param>
    /// <returns>Return the result</returns>
    public static IServiceCollection AddBearerAuthentication(this IServiceCollection service, JwtDto jwt)
    {
        // JWT
        var key = Encoding.UTF8.GetBytes(jwt.Signing);
        service.AddAuthentication(p =>
        {
            p.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            p.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(p =>
        {
            p.RequireHttpsMetadata = false;
            p.SaveToken = true;
            p.TokenValidationParameters = new TokenValidationParameters
            {
                ValidIssuer = jwt.Issuer,
                ValidAudience = jwt.Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero // so tokens expire exactly at token expiration time (instead of 5 minutes later)
            };
            p.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    if (context.Request.Query.ContainsKey("access_token"))
                    {
                        context.Token = context.Request.Query["access_token"];
                    }
                    return Task.CompletedTask;
                }
            };
        });

        // Add policy
        service.AddAuthorization(p =>
        {
            p.AddPolicy(Policy.Admin, q => q.RequireRole(McsgRole.SysAdmin, McsgRole.Admin, McsgRole.ContentAdmin));
        });

        return service;
    }

    /// <summary>
    /// Add distribution library
    /// </summary>
    /// <param name="service">Service</param>
    /// <param name="assembly">Assembly</param>
    /// <returns>Return the result</returns>
    public static IServiceCollection AddDistributionLibrary(this IServiceCollection service, Assembly assembly)
    {
        service.AddSingleton(p => { return new DistributeManager(assembly, service.BuildServiceProvider()); });

        return service;
    }

    /// <summary>
    /// Configures the maximum request body size and buffer size for IIS and Kestrel servers
    /// </summary>
    /// <param name="services">The IServiceCollection to configure</param>
    public static void ConfigureMaxRequestSizes(this IServiceCollection services)
    {
        var maxFileSize = 1024 * 1024 * 1024; // 1024MB
        var bufferSize = 10 * 1024 * 1024; // 10MB

        services.Configure<IISServerOptions>(p =>
        {
            p.MaxRequestBodySize = maxFileSize;
            p.MaxRequestBodyBufferSize = bufferSize;
        });

        services.Configure<KestrelServerOptions>(p =>
        {
            p.Limits.MaxRequestBodySize = maxFileSize;
            p.Limits.MaxRequestBufferSize = bufferSize;
        });

        services.Configure<FormOptions>(p =>
        {
            p.MultipartBodyLengthLimit = maxFileSize;
        });
    }

    /// <summary>
    /// AddSwaggerDocumentation
    /// </summary>
    /// <param name="services">The IServiceCollection to configure</param>
    /// <param name="scheme">Scheme</param>
    /// <returns>Returns the result</returns>
    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services, string scheme = "Bearer")
    {
        services.AddSwaggerGen(options =>
        {
            switch (scheme)
            {
                case "Bearer":
                    options.AddJwtSecurity();
                    break;

                case "ApiKey":
                    options.AddApiKeySecurity();
                    break;

                default:
                    break;
            }
        });

        return services;
    }

    #endregion
}
