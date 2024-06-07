#region Information
/*
 * Author       : Toan Nguyen Van
 * Email        : nvt87x@gmail.com
 * Phone        : +84 345 515 010
 * ------------------------------- *
 * Create       : 2024-Jan-21 07:03
 * Update       : 2024-Jan-21 07:03
 * Checklist    : 1.0
 * Status       : New
 */
#endregion

using Microsoft.Extensions.Options;

namespace Mcsg.Common.Core.Notifications;

using Dtos;
using Interfaces;
using SeedWork.Responses;
using static SeedWork.Dtos.ConnectionDto;

/// <summary>
/// Notification client
/// </summary>
public class NotificationClient : INotificationClient
{
    #region -- Implements --

    /// <summary>
    /// Set strategy
    /// </summary>
    /// <param name="strategy">Strategy</param>
    public void SetStrategy(INotificationStrategy strategy)
    {
        _strategy = strategy;
        _strategy.SetAuthSender(_auth);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="inf">Email information</param>
    /// <returns>Return the result</returns>
    public Task<SingleResponse> Handle(NotificationInfoDto inf)
    {
        ArgumentNullException.ThrowIfNull(_strategy, nameof(_strategy));

        return _strategy.Send(inf);
    }

    #endregion

    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="options">Options</param>
    public NotificationClient(IOptions<NotificationDto> options)
    {
        _auth = options.Value;
    }

    #endregion

    #region -- Fields --

    /// <summary>
    /// Strategy
    /// </summary>
    private INotificationStrategy? _strategy;

    /// <summary>
    /// Auth sender
    /// </summary>
    private NotificationDto _auth;

    #endregion
}
