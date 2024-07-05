using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Mcsg.Lib.Common.Web.Extensions.DependencyInjection
{
    using Lib.Common.Constants;
    using Mcsg.Common.SeedWork.Dtos;
    using Providers.AuthHandlers;
    using Providers.AuthHandlers.Scheme;

    public static class AuthenticationServiceCollectionExtensions
    {
        public static IServiceCollection AddBearerAuthentication(this IServiceCollection services, JwtDto jwt)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opts =>
            {
                opts.RequireHttpsMetadata = false;
                opts.SaveToken = true;
                opts.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidAudience = jwt.Audience,
                    ValidIssuer = jwt.Issuer,
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Signing ?? string.Empty)),
                };
                opts.Events = new JwtBearerEvents
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
