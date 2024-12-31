using MediatR;

namespace Mcsg.Identity.Api.Extensions;

using Common.SeedWork.Responses;
using Queries;

/// <summary>
/// DI extension
/// </summary>
public static class DiUserRecoveryExtension
{
    #region -- Methods --

    /// <summary>
    /// Add DI for include commands and queries
    /// </summary>
    /// <param name="p">MediatRServiceConfiguration</param>
    /// <param name="life">ServiceLifetime</param>
    public static void AddDiUserRecovery(this MediatRServiceConfiguration p, ServiceLifetime life = ServiceLifetime.Scoped)
    {
        p.AddUserRecoveryCommands(life);
        p.AddUserRecoveryQueries(life);
    }

    /// <summary>
    /// Add commands handler
    /// </summary>
    /// <param name="p">MediatRServiceConfiguration</param>
    /// <param name="life">ServiceLifetime</param>
    public static void AddUserRecoveryCommands(this MediatRServiceConfiguration p, ServiceLifetime life = ServiceLifetime.Scoped)
    {
        p.AddBehavior<IRequestHandler<UserRecoveryUpdateR, SingleResponse>, UserRecoveryUpdateH>(life);
    }

    /// <summary>
    /// Add queries handler
    /// </summary>
    /// <param name="p">MediatRServiceConfiguration</param>
    /// <param name="life">ServiceLifetime</param>
    public static void AddUserRecoveryQueries(this MediatRServiceConfiguration p, ServiceLifetime life = ServiceLifetime.Scoped)
    {
        p.AddBehavior<IRequestHandler<UserRecoverySearchR, SingleResponse>, UserRecoverySearchH>(life);
        p.AddBehavior<IRequestHandler<UserRecoveryViewR, SingleResponse>, UserRecoveryViewH>(life);
    }

    #endregion
}
