using MediatR;

namespace Mcsg.Media.Api.Extensions;

using Commands;
using Common.SeedWork.Responses;
using Requests;

/// <summary>
/// DI extension
/// </summary>
public static class DiPatchExtension
{
    #region -- Methods --

    /// <summary>
    /// Add DI for include commands and queries
    /// </summary>
    /// <param name="p">MediatRServiceConfiguration</param>
    /// <param name="life">ServiceLifetime</param>
    public static void AddDiPatch(this MediatRServiceConfiguration p, ServiceLifetime life = ServiceLifetime.Scoped)
    {
        p.AddPatchCommands(life);
        p.AddPatchQueries(life);
    }

    /// <summary>
    /// Add commands handler
    /// </summary>
    /// <param name="p">MediatRServiceConfiguration</param>
    /// <param name="life">ServiceLifetime</param>
    public static void AddPatchCommands(this MediatRServiceConfiguration p, ServiceLifetime life = ServiceLifetime.Scoped)
    {
        p.AddBehavior<IRequestHandler<PatchResizeImageR, SingleResponse>, PatchResizeImageH>(ServiceLifetime.Scoped);
        p.AddBehavior<IRequestHandler<PatchEncryptEmailR, SingleResponse>, PatchEncryptEmailH>(ServiceLifetime.Scoped);
    }

    /// <summary>
    /// Add queries handler
    /// </summary>
    /// <param name="p">MediatRServiceConfiguration</param>
    /// <param name="life">ServiceLifetime</param>
    public static void AddPatchQueries(this MediatRServiceConfiguration p, ServiceLifetime life = ServiceLifetime.Scoped)
    {
    }

    #endregion
}
