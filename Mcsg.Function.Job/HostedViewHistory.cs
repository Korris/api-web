using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Mcsg.Function.Job;

using Common.Core.Dtos;
using Common.Core.Enums;
using Common.Core.Extensions;
using Common.Domain;
using Common.Domain.Entities;
using Common.Models;
using Interfaces;
using static Common.SeedWork.Constants.Information;

/// <summary>
/// Hosted service https://www.c-sharpcorner.com/article/consuming-rabbitmq-messages-in-asp-net-core
/// </summary>
public class HostedViewHistory : BackgroundService
{
    #region -- Overrides --

    /// <summary>
    /// Execute async
    /// </summary>
    /// <param name="stoppingToken">Stopping token</param>
    /// <returns>A System.Threading.Tasks.Task that represents the long running operations</returns>
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += OnReceived;
        consumer.Shutdown += OnShutdown;
        consumer.Registered += OnRegistered;
        consumer.Unregistered += OnUnregistered;
        consumer.ConsumerCancelled += OnConsumerCancelled;

        using (var scope = _ss.CreateScope())
        {
            var st = scope.ServiceProvider.GetRequiredService<ISetting>();

            _channel.BasicConsume(st.NotificationQueueViewHistory, false, consumer);
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Dispose
    /// </summary>
    public override void Dispose()
    {
        _channel.Close();
        _connection.Close();
        base.Dispose();
    }

    #endregion

    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="ss">Service scope factory</param>
    /// <exception cref="ArgumentNullException"></exception>
    public HostedViewHistory(IServiceScopeFactory ss)
    {
        $"Initialize {nameof(HostedViewHistory)}".LogInfor();

        _ss = ss ?? throw new ArgumentNullException(nameof(ss));

        using (var scope = _ss.CreateScope())
        {
            var st = scope.ServiceProvider.GetRequiredService<ISetting>();

            _connection = st.Queue.CreateConnection();
            "_connection created".LogInfor();

            _channel = _connection.CreateModel();
            "_channel created".LogInfor();

            _channel.ExchangeDeclare(st.NotificationExchange, ExchangeType.Direct);
            _channel.QueueDeclare(st.NotificationQueueViewHistory, false, false, false, null);
            _channel.QueueBind(st.NotificationQueueViewHistory, st.NotificationExchange, st.NotificationQueueViewHistory, null);
            _channel.BasicQos(0, 1, false);

            _connection.ConnectionShutdown += OnConnectionShutdown;

            $"Finished {nameof(HostedViewHistory)}".LogInfor();
        }
    }

    /// <summary>
    /// Handle message
    /// </summary>
    /// <param name="message">Message</param>
    private async void HandleMessage(string message)
    {
        $"Consumer received {message}".LogInfor("JSON");

        var msg = message.ToInstNull<QueueMessageDto>();
        if (msg == null)
        {
            $"{I003} {message}".LogInfor();
            return;
        }

        using (var scope = _ss.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<IMcsgContext>();
            var payload = JsonConvert.DeserializeObject<ViewHistoryData>(msg.Payload);

            if (payload == null)
            {
                return;
            }

            var check = await context.ViewHistories
                .Where(p => p.EntityId == payload.EntityId
                    && p.UsedId == payload.UserId
                    && p.EntityType == payload.EntityType
                    && p.IpAddress == payload.IdAddress)
                .Select(p => p.Id)
                .FirstOrDefaultAsync();
            if (check != Guid.Empty)
            {
                return;
            }

            var history = new ViewHistory
            {
                EntityType = payload.EntityType,
                EntityId = payload.EntityId,
                CreatedOn = DateTime.UtcNow,
                IpAddress = payload.IdAddress,
                SubType = payload.SubType,
                UsedId = payload.UserId
            };
            await context.ViewHistories.AddAsync(history);
            await context.SaveChangesAsync(default);

            var smartLookupData = new SmartCountEntityData
            {
                ActionType = ActionType.View,
                EntityId = payload.EntityId,
                EntityType = payload.EntityType,
                SubType = payload.SubType,
                IsRemove = false,
            };
            var viewHistoryCountService = scope.ServiceProvider.GetRequiredService<ICountService<ViewHistory, ViewHistory>>();
            await viewHistoryCountService.RunQueue(smartLookupData);
        }
    }

    #endregion

    #region -- Events --

    /// <summary>
    /// On connection shutdown
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Event</param>
    private void OnConnectionShutdown(object? sender, ShutdownEventArgs e)
    {
        $"Connection shutdown {e.ReplyText}".LogInfor();
    }

    /// <summary>
    /// On received
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Event</param>
    private void OnReceived(object? sender, BasicDeliverEventArgs e)
    {
        // Received message
        var content = Encoding.UTF8.GetString(e.Body.ToArray());

        // Handle the received message
        HandleMessage(content);

        _channel.BasicAck(e.DeliveryTag, false);
    }

    /// <summary>
    /// On shutdown
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Event</param>
    private void OnShutdown(object? sender, ShutdownEventArgs e)
    {
        $"Consumer shutdown {e.ReplyText}".LogInfor();
    }

    /// <summary>
    /// On registered
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Event</param>
    private void OnRegistered(object? sender, ConsumerEventArgs e)
    {
        $"Consumer registered {e.ConsumerTags}".LogInfor();
    }

    /// <summary>
    /// On unregistered
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Event</param>
    private void OnUnregistered(object? sender, ConsumerEventArgs e)
    {
        $"Consumer unregistered {e.ConsumerTags}".LogInfor();
    }

    /// <summary>
    /// On consumer cancelled
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Event</param>
    private void OnConsumerCancelled(object? sender, ConsumerEventArgs e)
    {
        $"Consumer cancelled {e.ConsumerTags}".LogInfor();
    }

    #endregion

    #region -- Fields --

    /// <summary>
    /// Service scope factory
    /// </summary>
    private readonly IServiceScopeFactory _ss;

    /// <summary>
    /// Connection
    /// </summary>
    private IConnection _connection;

    /// <summary>
    /// Channel
    /// </summary>
    private IModel _channel;

    #endregion
}
