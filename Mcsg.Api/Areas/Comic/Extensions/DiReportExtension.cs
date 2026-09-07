using MediatR;

namespace Mcsg.Api.Areas.Comic.Extensions;

using Mcsg.Api.Areas.Comic.Commands;
using Common.SeedWork.Responses;
using Mcsg.Api.Areas.Comic.Requests;

/// <summary>
/// DI extension
/// </summary>
public static class DiReportExtension
{
    #region -- Methods --

    /// <summary>
    /// Add DI for include commands and queries
    /// </summary>
    /// <param name="p">MediatRServiceConfiguration</param>
    /// <param name="life">ServiceLifetime</param>
    public static void AddDiReport(this MediatRServiceConfiguration p, ServiceLifetime life = ServiceLifetime.Scoped)
    {
        p.AddReportCommands(life);
        p.AddReportQueries(life);
    }

    /// <summary>
    /// Add commands handler
    /// </summary>
    /// <param name="p">MediatRServiceConfiguration</param>
    /// <param name="life">ServiceLifetime</param>
    public static void AddReportCommands(this MediatRServiceConfiguration p, ServiceLifetime life = ServiceLifetime.Scoped)
    {
        p.AddBehavior<IRequestHandler<ReportCreateR, SingleResponse>, ReportCreateH>(life);
    }

    /// <summary>
    /// Add queries handler
    /// </summary>
    /// <param name="p">MediatRServiceConfiguration</param>
    /// <param name="life">ServiceLifetime</param>
    public static void AddReportQueries(this MediatRServiceConfiguration p, ServiceLifetime life = ServiceLifetime.Scoped)
    {
    }

    #endregion
}
