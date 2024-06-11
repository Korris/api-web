using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Mcsg.Function.Job;

using Common.Core.Dtos;
using Common.Core.Extensions;
using Interfaces;
using Lib.Common.Models;
using Mcsg.Function.Job.Services;
using static Common.SeedWork.Constants.Information;

/// <summary>
/// Hosted service https://www.c-sharpcorner.com/article/consuming-rabbitmq-messages-in-asp-net-core
/// </summary>
public class SyncDataFunction : BackgroundService
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

            _channel.BasicConsume(st.NotificationQueueSyncData, false, consumer);
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
    public SyncDataFunction(IServiceScopeFactory ss)
    {
        $"Initialize {nameof(SyncDataFunction)}".LogInfor();

        _ss = ss ?? throw new ArgumentNullException(nameof(ss));

        using (var scope = _ss.CreateScope())
        {
            var st = scope.ServiceProvider.GetRequiredService<ISetting>();

            _connection = st.Queue.CreateConnection();
            "_connection created".LogInfor();

            _channel = _connection.CreateModel();
            "_channel created".LogInfor();

            _channel.ExchangeDeclare(st.NotificationExchange, ExchangeType.Direct);
            _channel.QueueDeclare(st.NotificationQueueSyncData, false, false, false, null);
            _channel.QueueBind(st.NotificationQueueSyncData, st.NotificationExchange, st.NotificationQueueSyncData, null);
            _channel.BasicQos(0, 1, false);

            _connection.ConnectionShutdown += OnConnectionShutdown;

            $"Finished {nameof(SyncDataFunction)}".LogInfor();
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
            var syncDataService = scope.ServiceProvider.GetRequiredService<ISyncDataService>();

            var syncData = JsonConvert.DeserializeObject<SyncData>(msg.Payload);

            switch (syncData.TargetDb)
            {
                case SyncTargetDb.WALLETDB:
                    {
                        if (syncData.TargetEntity == SyncTargetEntity.WALLET_USER_INFO)
                        {
                            await syncDataService.SyncWalletUserInfoAsync(syncData);
                        }
                        if (syncData.TargetEntity == SyncTargetEntity.WALLET_USER_REWARD)
                        {
                            await syncDataService.SyncWalletUserRewardAsync(syncData);
                        }
                        if (syncData.TargetEntity == SyncTargetEntity.WALLET_USER_BUY_PREMIUM)
                        {
                            await syncDataService.SyncUserPremiumAsync(syncData);
                        }
                        if (syncData.TargetEntity == SyncTargetEntity.WALLET_USER_BUY_CHAPTER)
                        {
                            await syncDataService.SyncUserBuyChapterAsync(syncData);
                        }
                        if (syncData.TargetEntity == SyncTargetEntity.WALLET_USER_BUY_SERIES)
                        {
                            await syncDataService.SyncUserBuySeriesAsync(syncData);
                        }
                        break;
                    }

                default:
                    {
                        break;
                    }
            }
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
