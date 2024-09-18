using MediatR;

namespace Mcsg.Identity.Api.Extensions;

using Commands;
using Common.SeedWork.Responses;
using Queries;
using Requests;

/// <summary>
/// DI extension
/// </summary>
public static class DiFeedbackExtension
{
    #region -- Methods --

    /// <summary>
    /// Add DI for include commands and queries
    /// </summary>
    /// <param name="p">MediatRServiceConfiguration</param>
    /// <param name="life">ServiceLifetime</param>
    public static void AddDiFeedback(this MediatRServiceConfiguration p, ServiceLifetime life = ServiceLifetime.Scoped)
    {
        p.AddFeedbackCommands(life);
        p.AddFeedbackQueries(life);
    }

    /// <summary>
    /// Add commands handler
    /// </summary>
    /// <param name="p">MediatRServiceConfiguration</param>
    /// <param name="life">ServiceLifetime</param>
    public static void AddFeedbackCommands(this MediatRServiceConfiguration p, ServiceLifetime life = ServiceLifetime.Scoped)
    {
        p.AddBehavior<IRequestHandler<FeedbackCreateR, SingleResponse>, FeedbackCreateH>(life);
    }

    /// <summary>
    /// Add queries handler
    /// </summary>
    /// <param name="p">MediatRServiceConfiguration</param>
    /// <param name="life">ServiceLifetime</param>
    public static void AddFeedbackQueries(this MediatRServiceConfiguration p, ServiceLifetime life = ServiceLifetime.Scoped)
    {
        p.AddBehavior<IRequestHandler<FeedbackSearchR, SingleResponse>, FeedbackSearchH>(life);
    }

    #endregion
}
