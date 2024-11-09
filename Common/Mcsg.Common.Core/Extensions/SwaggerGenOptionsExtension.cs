using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Mcsg.Common.Core.Extensions;

/// <summary>
/// SwaggerGenOptions extension for using [this SwaggerGenOptions] only
/// </summary>
public static class SwaggerGenOptionsExtension
{
    /// <summary>
    /// Add JWT security
    /// </summary>
    /// <param name="options">Options</param>
    public static void AddJwtSecurity(this SwaggerGenOptions options)
    {
        var scheme = new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please enter a valid token",
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Scheme = "Bearer"
        };
        options.AddSecurityDefinition("Bearer", scheme);

        var reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        };
        var requirement = new OpenApiSecurityRequirement {
            {
                new OpenApiSecurityScheme {
                    Reference = reference
                },
                Array.Empty<string>()
            }
        };
        options.AddSecurityRequirement(requirement);
    }

    /// <summary>
    /// Add API key security
    /// </summary>
    /// <param name="options">Options</param>
    public static void AddApiKeySecurity(this SwaggerGenOptions options)
    {
        var scheme = new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "ApiKey must appear in header",
            Name = "X-Api-Key",
            Type = SecuritySchemeType.ApiKey,
            Scheme = "ApiKey"
        };
        options.AddSecurityDefinition("ApiKey", scheme);

        var reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "ApiKey"
        };
        var requirement = new OpenApiSecurityRequirement {
            {
                new OpenApiSecurityScheme {
                    Reference = reference,
                    In = ParameterLocation.Header
                },
                Array.Empty<string>()
            }
        };
        options.AddSecurityRequirement(requirement);
    }
}
