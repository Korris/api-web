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

/// <summary>
/// Interface notification client
/// </summary>
public interface INotificationClient
{
    #region -- Methods --

    /// <summary>
    /// Set strategy
    /// </summary>
    /// <param name="strategy">Strategy</param>
    public void SetStrategy(INotificationStrategy strategy);

    /// <summary>
    /// Handle
    /// </summary>
    /// <param name="inf">Email information</param>
    /// <returns>Return the result</returns>
    Task<SingleResponse> Handle(NotificationInfoDto inf);

    #endregion
}
