namespace Mcsg.Api.Areas.Game.Extensions;

using Mcsg.Api.Areas.Game.Attributes;
using Mcsg.Api.Areas.Game.Interfaces;
using Mcsg.Api.Areas.Game.Services;

/// <summary>
/// DI registration for the Game area (called from Program.cs)
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGameServices(this IServiceCollection services)
    {
        services.AddScoped<MediaOnlyAttribute>();
        services.AddScoped<IGameFileService, GameFileService>();
        services.AddScoped<IGameReactService, GameReactService>();
        services.AddScoped<IGameCommentService, GameCommentService>();
        services.AddScoped<IGamePostService, GamePostService>();
        return services;
    }
}
