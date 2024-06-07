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

namespace Mcsg.Common.Core.Interfaces;

using Dtos;
using SeedWork.Responses;
using static SeedWork.Dtos.ConnectionDto;

/// <summary>
/// Interface notification<br/>
/// https://refactoring.guru/design-patterns/strategy/csharp/example
/// </summary>
public interface INotificationStrategy
{
    #region -- Methods --

    /// <summary>
    /// Set auth sender
    /// </summary>
    /// <param name="auth">Auth sender</param>
    void SetAuthSender(NotificationDto auth);

    /// <summary>
    /// Send
    /// </summary>
    /// <param name="inf">Email information</param>
    /// <returns>Return the result</returns>
    Task<SingleResponse> Send(NotificationInfoDto inf);

    #endregion
}
