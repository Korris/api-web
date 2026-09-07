using MediatR;

namespace Mcsg.Api.Areas.Identity.Extensions;

using Commands;
using Common.SeedWork.Responses;
using Queries;
using Requests;

/// <summary>
/// DI extension
/// </summary>
public static class DiUserExtension
{
    #region -- Methods --

    /// <summary>
    /// Add DI for include commands and queries
    /// </summary>
    /// <param name="p">MediatRServiceConfiguration</param>
    /// <param name="life">ServiceLifetime</param>
    public static void AddDiUser(this MediatRServiceConfiguration p, ServiceLifetime life = ServiceLifetime.Scoped)
    {
        p.AddUserCommands(life);
        p.AddUserQueries(life);
    }

    /// <summary>
    /// Add commands handler
    /// </summary>
    /// <param name="p">MediatRServiceConfiguration</param>
    /// <param name="life">ServiceLifetime</param>
    public static void AddUserCommands(this MediatRServiceConfiguration p, ServiceLifetime life = ServiceLifetime.Scoped)
    {
        p.AddBehavior<IRequestHandler<UserNameUpdateR, SingleResponse>, UserNameUpdateH>(life);
        p.AddBehavior<IRequestHandler<UserSyncToAnaR, SingleResponse>, UserSyncToAnaH>(life);
    }

    /// <summary>
    /// Add queries handler
    /// </summary>
    /// <param name="p">MediatRServiceConfiguration</param>
    /// <param name="life">ServiceLifetime</param>
    public static void AddUserQueries(this MediatRServiceConfiguration p, ServiceLifetime life = ServiceLifetime.Scoped)
    {
        p.AddBehavior<IRequestHandler<UserHistoryGetLatestR, SingleResponse>, UserHistoryGetLatestH>(life);
    }

    #endregion
}
