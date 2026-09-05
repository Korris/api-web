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

using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace Mcsg.Common.Core.Extensions;

using Dtos;
using SeedWork.Interfaces;
using static SeedWork.Constants.Information;

/// <summary>
/// ISetting extension for using [this ISetting] only
/// </summary>
public static class ISettingExtension
{
    #region -- Methods --

    /// <summary>
    /// Send message to queue
    /// </summary>
    /// <param name="setting">Setting</param>
    /// <param name="exchange">Exchange</param>
    /// <param name="queueName">Queue name</param>
    /// <param name="message">Message data</param>
    public static void SendMessageToQueue(this ISettingBase setting, string exchange, string queueName, QueueMessageDto message)
    {
        if (message == null)
        {
            return;
        }

        using (var connection = setting.Queue.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            I100.LogInfor();

            channel.QueueDeclare(queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            message.Prefix = setting.Prefix;
            message.Environment = setting.Environment;

            var json = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(json);
            json.LogInfor("JSON");

            channel.BasicPublish(exchange: exchange,
                routingKey: queueName,
                basicProperties: null,
                body: body);

            I101.LogInfor();
        }
    }

    /// <summary>
    /// Writes a log event at the information level
    /// </summary>
    /// <param name="setting">Setting</param>
    public static void LogInfor(this ISettingBase setting)
    {
        // Log information about the system environment
        var st = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
        var json = JsonConvert.SerializeObject(setting, Formatting.Indented, st);
        $"System environments: {json}".LogInfor();
    }

    #endregion
}
