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

using RabbitMQ.Client;

namespace Mcsg.Common.Core.Extensions;

using static SeedWork.Dtos.ConnectionDto;

/// <summary>
/// Dto extension for using [this *Dto] only
/// </summary>
public static class DtoExtension
{
    #region -- Methods --

    /// <summary>
    /// Create a connection to one of the endpoints provided by the IEndpointResolver
    /// returned by the EndpointResolverFactory. By default the configured hostname and port are used
    /// </summary>
    /// <param name="setting">Queue setting</param>
    /// <returns>Return main interface to an AMQP connection</returns>
    public static IConnection CreateConnection(this QueueDto setting)
    {
        var factory = new ConnectionFactory
        {
            HostName = setting.Host,
            Port = setting.Port,
            UserName = setting.UserName,
            Password = setting.Password,
            VirtualHost = setting.VirtualHost
        };

        return factory.CreateConnection();
    }

    #endregion
}
