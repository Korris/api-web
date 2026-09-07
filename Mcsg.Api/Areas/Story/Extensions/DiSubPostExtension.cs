#region Information
/*
 * Author       : Toan Nguyen Van
 * Email        : nvt87x@gmail.com
 * Phone        : +84 345 515 010
 * ------------------------------- *
 * Create       : 2024-Jan-21 08:37
 * Update       : 2024-Jan-21 08:37
 * Checklist    : 1.0
 * Status       : New
 */
#endregion

using MediatR;

namespace Mcsg.Api.Areas.Story.Extensions;

using Mcsg.Api.Areas.Story.Commands;
using Common.SeedWork.Responses;
using Mcsg.Api.Areas.Story.Queries;
using Mcsg.Api.Areas.Story.Requests;

/// <summary>
/// DI extension
/// </summary>
public static class DiSubPostExtension
{
    #region -- Methods --

    /// <summary>
    /// Add DI for include commands and queries
    /// </summary>
    /// <param name="p">MediatRServiceConfiguration</param>
    /// <param name="life">ServiceLifetime</param>
    public static void AddDiSubPost(this MediatRServiceConfiguration p, ServiceLifetime life = ServiceLifetime.Scoped)
    {
        p.AddSubPostCommands(life);
        p.AddSubPostQueries(life);
    }

    /// <summary>
    /// Add commands handler
    /// </summary>
    /// <param name="p">MediatRServiceConfiguration</param>
    /// <param name="life">ServiceLifetime</param>
    public static void AddSubPostCommands(this MediatRServiceConfiguration p, ServiceLifetime life = ServiceLifetime.Scoped)
    {
        p.AddBehavior<IRequestHandler<SubPostDataPatchR, SingleResponse>, SubPostDataPatchH>(life);
        p.AddBehavior<IRequestHandler<SubPostSyncToAnaR, SingleResponse>, SubPostSyncToAnaH>(life);
    }

    /// <summary>
    /// Add queries handler
    /// </summary>
    /// <param name="p">MediatRServiceConfiguration</param>
    /// <param name="life">ServiceLifetime</param>
    public static void AddSubPostQueries(this MediatRServiceConfiguration p, ServiceLifetime life = ServiceLifetime.Scoped)
    {
        p.AddBehavior<IRequestHandler<SubPostSearchR, SingleResponse>, SubPostSearchH>(life);
    }

    #endregion
}
