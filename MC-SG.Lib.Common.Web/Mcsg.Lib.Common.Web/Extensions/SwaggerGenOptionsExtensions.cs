using Mcsg.Lib.Common.Constants;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;

namespace Mcsg.Lib.Common.Web.Extensions
{
    public static class SwaggerGenOptionsExtensions
    {
        public static void AddJwtSecurity(this SwaggerGenOptions swaggerGenOptions)
        {
            swaggerGenOptions.AddSecurityDefinition(AuthenticationSchemes.JwtScheme, new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter a valid token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = AuthenticationSchemes.JwtScheme
            });
            swaggerGenOptions.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = AuthenticationSchemes.JwtScheme
                            }
                        },
                        Array.Empty<string>()
                    }
                });
        }

        public static void AddApiKeySecurity(this SwaggerGenOptions swaggerGenOptions)
        {
            swaggerGenOptions.AddSecurityDefinition(AuthenticationSchemes.ApiKeyScheme, new OpenApiSecurityScheme
            {
                Description = "ApiKey must appear in header",
                Type = SecuritySchemeType.ApiKey,
                Name = HttpHeaders.ApiKey,
                In = ParameterLocation.Header,
                Scheme = AuthenticationSchemes.ApiKeyScheme
            });

            swaggerGenOptions.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
               {
                    new OpenApiSecurityScheme()
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = AuthenticationSchemes.ApiKeyScheme
                        },
                        In = ParameterLocation.Header
                    }, Array.Empty<string>()
                }
            });


        }
    }
}
