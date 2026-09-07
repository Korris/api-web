using MediatR;

namespace Mcsg.Api.Areas.Document.Extensions;

using Common.SeedWork.Responses;
using Mcsg.Api.Areas.Document.Queries;
using Mcsg.Api.Areas.Document.Requests;

/// <summary>
/// DI extension
/// </summary>
public static class DiResourceExtension
{
    #region -- Methods --

    /// <summary>
    /// Add DI for include commands and queries
    /// </summary>
    /// <param name="p">MediatRServiceConfiguration</param>
    /// <param name="life">ServiceLifetime</param>
    public static void AddDiResource(this MediatRServiceConfiguration p, ServiceLifetime life = ServiceLifetime.Scoped)
    {
        p.AddResourceCommands(life);
        p.AddResourceQueries(life);
    }

    /// <summary>
    /// Add commands handler
    /// </summary>
    /// <param name="p">MediatRServiceConfiguration</param>
    /// <param name="life">ServiceLifetime</param>
    public static void AddResourceCommands(this MediatRServiceConfiguration p, ServiceLifetime life = ServiceLifetime.Scoped)
    {
    }

    /// <summary>
    /// Add queries handler
    /// </summary>
    /// <param name="p">MediatRServiceConfiguration</param>
    /// <param name="life">ServiceLifetime</param>
    public static void AddResourceQueries(this MediatRServiceConfiguration p, ServiceLifetime life = ServiceLifetime.Scoped)
    {
        p.AddBehavior<IRequestHandler<ResourceSearchR, SingleResponse>, ResourceSearchH>(life);
        p.AddBehavior<IRequestHandler<ResourceViewR, SingleResponse>, ResourceViewH>(life);
    }

    #endregion
}
