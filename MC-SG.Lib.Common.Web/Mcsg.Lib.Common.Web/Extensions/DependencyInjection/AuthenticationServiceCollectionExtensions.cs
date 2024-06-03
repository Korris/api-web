using Mcsg.Lib.Common.Constants;
using Mcsg.Lib.Common.Models;
using Mcsg.Lib.Common.Web.Providers.AuthHandlers;
using Mcsg.Lib.Common.Web.Providers.AuthHandlers.Scheme;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;

namespace Mcsg.Lib.Common.Web.Extensions.DependencyInjection
{
    public static class AuthenticationServiceCollectionExtensions
    {
        public static IServiceCollection AddBearerAuthentication(this IServiceCollection services, ConfigurationManager configuration)
        {
            services.Configure<JwtSetting>(configuration.GetSection("JWT"));

            var setting = new JwtSetting();
            configuration.GetSection("JWT").Bind(setting);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opts =>
            {
                opts.RequireHttpsMetadata = false;
                opts.SaveToken = true;
                opts.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidAudience = setting.Audience,
                    ValidIssuer = setting.Issuer,
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(setting.Key ?? string.Empty)),
                };
                opts.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        if (context.Request.Query.ContainsKey("access_token"))
                        {
                            context.Token = context.Request.Query["access_token"];
                        }
                        return System.Threading.Tasks.Task.CompletedTask;
                    }
                };
            });
            services.AddAuthorization();

            return services;
        }

        public static IServiceCollection AddApiKeyAuthentication(this IServiceCollection services, ConfigurationManager configuration)
        {
            services.AddAuthentication(AuthenticationSchemes.ApiKeyScheme)
                .AddScheme<ApiKeySchemeOptions, ApiKeySchemeHandler>(AuthenticationSchemes.ApiKeyScheme, options =>
                {
                    options.HeaderName = HttpHeaders.ApiKey;
                    options.AuthKey = configuration.GetValue<string>("APIKey");
                });

            services.AddAuthorization();

            return services;
        }
    }
}
