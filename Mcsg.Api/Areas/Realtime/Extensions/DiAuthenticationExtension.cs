using MediatR;

namespace Mcsg.Api.Areas.Realtime.Extensions;

using Mcsg.Api.Areas.Realtime.Commands;
using Common.SeedWork.Responses;
using Mcsg.Api.Areas.Realtime.Requests;

/// <summary>
/// DI extension for Realtime Authentication MediatR handlers
/// </summary>
public static class DiAuthenticationExtension
{
    #region -- Methods --

    /// <summary>
    /// Add DI for authentication commands and queries
    /// </summary>
    public static void AddDiRealtimeAuthentication(this MediatRServiceConfiguration p, ServiceLifetime life = ServiceLifetime.Scoped)
    {
        p.AddRealtimeAuthenticationCommands(life);
    }

    /// <summary>
    /// Add command handlers
    /// </summary>
    public static void AddRealtimeAuthenticationCommands(this MediatRServiceConfiguration p, ServiceLifetime life = ServiceLifetime.Scoped)
    {
        p.AddBehavior<IRequestHandler<AuthenticationForceLogoutR, SingleResponse>, AuthenticationForceLogoutH>(life);
        p.AddBehavior<IRequestHandler<AuthenticationResetPasswordR, SingleResponse>, AuthenticationResetPasswordH>(life);
        p.AddBehavior<IRequestHandler<AuthenticationVerifyOtpR, SingleResponse>, AuthenticationVerifyOtpH>(life);
    }

    #endregion
}
