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

namespace Mcsg.Common.Core.Notifications;

using Dtos;
using Interfaces;
using SeedWork.Responses;
using static SeedWork.Dtos.ConnectionDto;

/// <summary>
/// Notification strategy
/// </summary>
public class NotificationStrategy : INotificationStrategy
{
    #region -- Implements --

    /// <summary>
    /// Set auth sender
    /// </summary>
    /// <param name="auth">Auth sender</param>
    public void SetAuthSender(NotificationDto auth)
    {
        _auth = auth;
    }

    /// <summary>
    /// Send
    /// </summary>
    /// <param name="inf">Email information</param>
    /// <returns>Return the result</returns>
    public virtual Task<SingleResponse> Send(NotificationInfoDto inf)
    {
        throw new NotImplementedException();
    }

    #endregion

    #region -- Fields --

    /// <summary>
    /// Auth sender
    /// </summary>
    protected NotificationDto? _auth { get; set; }

    #endregion
}
