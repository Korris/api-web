using MediatR;

namespace Mcsg.Api.Areas.Social.Extensions;

using Mcsg.Api.Areas.Social.Commands;
using Common.SeedWork.Responses;
using Mcsg.Api.Areas.Social.Requests;

/// <summary>
/// DI extension
/// </summary>
public static class DiTagExtension
{
    #region -- Methods --

    /// <summary>
    /// Add DI for include commands and queries
    /// </summary>
    /// <param name="p">MediatRServiceConfiguration</param>
    /// <param name="life">ServiceLifetime</param>
    public static void AddDiTag(this MediatRServiceConfiguration p, ServiceLifetime life = ServiceLifetime.Scoped)
    {
        p.AddTagCommands(life);
        p.AddTagQueries(life);
    }

    /// <summary>
    /// Add commands handler
    /// </summary>
    /// <param name="p">MediatRServiceConfiguration</param>
    /// <param name="life">ServiceLifetime</param>
    public static void AddTagCommands(this MediatRServiceConfiguration p, ServiceLifetime life = ServiceLifetime.Scoped)
    {
    }

    /// <summary>
    /// Add queries handler
    /// </summary>
    /// <param name="p">MediatRServiceConfiguration</param>
    /// <param name="life">ServiceLifetime</param>
    public static void AddTagQueries(this MediatRServiceConfiguration p, ServiceLifetime life = ServiceLifetime.Scoped)
    {
        p.AddBehavior<IRequestHandler<TagPopularForPostsR, SingleResponse>, TagPopularForPostsH>(life);
    }

    #endregion
}
