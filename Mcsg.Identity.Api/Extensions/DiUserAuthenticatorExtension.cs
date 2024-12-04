using MediatR;

namespace Mcsg.Identity.Api.Extensions;

using Commands;
using Common.SeedWork.Responses;

/// <summary>
/// DI extension
/// </summary>
public static class DiUserAuthenticatorExtension
{
    #region -- Methods --

    /// <summary>
    /// Add DI for include commands and queries
    /// </summary>
    /// <param name="p">MediatRServiceConfiguration</param>
    /// <param name="life">ServiceLifetime</param>
    public static void AddDiUserAuthenticator(this MediatRServiceConfiguration p, ServiceLifetime life = ServiceLifetime.Scoped)
    {
        p.AddUserAuthenticatorCommands(life);
        p.AddUserAuthenticatorQueries(life);
    }

    /// <summary>
    /// Add commands handler
    /// </summary>
    /// <param name="p">MediatRServiceConfiguration</param>
    /// <param name="life">ServiceLifetime</param>
    public static void AddUserAuthenticatorCommands(this MediatRServiceConfiguration p, ServiceLifetime life = ServiceLifetime.Scoped)
    {
        p.AddBehavior<IRequestHandler<UserAuthenticatorCreateR, SingleResponse>, UserAuthenticatorCreateH>(life);
        p.AddBehavior<IRequestHandler<UserAuthenticatorUpdateR, SingleResponse>, UserAuthenticatorUpdateH>(life);
        p.AddBehavior<IRequestHandler<UserAuthenticatorDeleteR, SingleResponse>, UserAuthenticatorDeleteH>(life);
    }

    /// <summary>
    /// Add queries handler
    /// </summary>
    /// <param name="p">MediatRServiceConfiguration</param>
    /// <param name="life">ServiceLifetime</param>
    public static void AddUserAuthenticatorQueries(this MediatRServiceConfiguration p, ServiceLifetime life = ServiceLifetime.Scoped)
    {
    }

    #endregion
}
