using MediatR;

namespace Mcsg.Social.Api.Extensions;

using Commands;
using Common.SeedWork.Responses;
using Queries;
using Requests;

/// <summary>
/// DI extension
/// </summary>
public static class DiPostFavoriteExtension
{
    #region -- Methods --

    /// <summary>
    /// Add DI for include commands and queries
    /// </summary>
    /// <param name="p">MediatRServiceConfiguration</param>
    /// <param name="life">ServiceLifetime</param>
    public static void AddDiPostFavorite(this MediatRServiceConfiguration p, ServiceLifetime life = ServiceLifetime.Scoped)
    {
        p.AddPostFavoriteCommands(life);
        p.AddPostFavoriteQueries(life);
    }

    /// <summary>
    /// Add commands handler
    /// </summary>
    /// <param name="p">MediatRServiceConfiguration</param>
    /// <param name="life">ServiceLifetime</param>
    public static void AddPostFavoriteCommands(this MediatRServiceConfiguration p, ServiceLifetime life = ServiceLifetime.Scoped)
    {
        p.AddBehavior<IRequestHandler<PostFavoriteUpdateR, SingleResponse>, PostFavoriteUpdateH>(life);
    }

    /// <summary>
    /// Add queries handler
    /// </summary>
    /// <param name="p">MediatRServiceConfiguration</param>
    /// <param name="life">ServiceLifetime</param>
    public static void AddPostFavoriteQueries(this MediatRServiceConfiguration p, ServiceLifetime life = ServiceLifetime.Scoped)
    {
        p.AddBehavior<IRequestHandler<FavoriteViewR, SingleResponse>, FavoriteViewH>(life);
        p.AddBehavior<IRequestHandler<FavoriteSearchR, SingleResponse>, FavoriteSearchH>(life);
    }

    #endregion
}
