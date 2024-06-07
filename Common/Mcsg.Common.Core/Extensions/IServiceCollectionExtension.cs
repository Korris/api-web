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

using Microsoft.Extensions.DependencyInjection;

namespace Mcsg.Common.Core.Extensions;

using Interfaces;
using Notifications;
using static SeedWork.Dtos.ConnectionDto;

/// <summary>
/// IServiceCollection extension for using [this IServiceCollection] only
/// </summary>
public static class IServiceCollectionExtension
{
    #region -- Methods --

    /// <summary>
    /// Add notification
    /// </summary>
    /// <param name="service">Service</param>
    /// <param name="action">Configure action</param>
    /// <returns>Return the result</returns>
    public static IServiceCollection AddNotification(this IServiceCollection service, Action<NotificationDto> action)
    {
        service.Configure(action);
        service.AddSingleton<INotificationClient, NotificationClient>();

        return service;
    }

    #endregion
}
