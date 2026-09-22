namespace Mcsg.Api.Areas.TapShow.Extensions;

using Mcsg.Api.Areas.TapShow.Attributes;
using Mcsg.Api.Areas.TapShow.Interfaces;
using Mcsg.Api.Areas.TapShow.Services;

/// <summary>
/// DI registration for the TapShow area (called from Program.cs)
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTapShowServices(this IServiceCollection services)
    {
        services.AddScoped<MediaOnlyAttribute>();
        services.AddScoped<ITapShowFileService, TapShowFileService>();
        services.AddScoped<ITapShowResourceService, TapShowResourceService>();
        services.AddScoped<ITapShowReactService, TapShowReactService>();
        services.AddScoped<ITapShowCommentService, TapShowCommentService>();
        services.AddScoped<ITapShowPostService, TapShowPostService>();
        services.AddScoped<ITapShowChapterService, TapShowChapterService>();
        services.AddScoped<ITapShowSegmentService, TapShowSegmentService>();
        services.AddScoped<ITapShowCharacterService, TapShowCharacterService>();
        return services;
    }
}
