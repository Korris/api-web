using Dapper;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Mcsg.Function.Job;

using Common.Core.Dtos;
using Common.Core.Enums;
using Common.Core.Extensions;
using Common.Domain.Entities;
using Interfaces;
using Lib.Data.Repositories;
using static Common.SeedWork.Constants.Information;

/// <summary>
/// Hosted service https://www.c-sharpcorner.com/article/consuming-rabbitmq-messages-in-asp-net-core
/// </summary>
public class HostedSmartLoopkup : BackgroundService
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

            _channel.BasicConsume(st.NotificationQueueSmartLookup, false, consumer);
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
    public HostedSmartLoopkup(IServiceScopeFactory ss)
    {
        $"Initialize {nameof(HostedSmartLoopkup)}".LogInfor();

        _ss = ss ?? throw new ArgumentNullException(nameof(ss));

        using (var scope = _ss.CreateScope())
        {
            var st = scope.ServiceProvider.GetRequiredService<ISetting>();

            _connection = st.Queue.CreateConnection();
            "_connection created".LogInfor();

            _channel = _connection.CreateModel();
            "_channel created".LogInfor();

            _channel.ExchangeDeclare(st.NotificationExchange, ExchangeType.Direct);
            _channel.QueueDeclare(st.NotificationQueueSmartLookup, false, false, false, null);
            _channel.QueueBind(st.NotificationQueueSmartLookup, st.NotificationExchange, st.NotificationQueueSmartLookup, null);
            _channel.BasicQos(0, 1, false);

            _connection.ConnectionShutdown += OnConnectionShutdown;

            $"Finished {nameof(HostedSmartLoopkup)}".LogInfor();
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
            var service = scope.ServiceProvider.GetRequiredService<IRepository<SmartLookup>>();
            var payload = JsonConvert.DeserializeObject<SmartLookupDto>(msg.Payload);

            switch (payload.KeywordType)
            {
                case LookupKeywordType.People:
                    await service.Connection.ExecuteAsync(UpdateSmartLookupPeopleCommand, new { Name = payload.ProfileName, KeywordType = (int)payload.KeywordType });
                    break;
                case LookupKeywordType.Tag:
                    if (payload.Tags.Any())
                    {
                        foreach (var tag in payload.Tags)
                        {
                            var countTagPost = await service.Connection
                                .QueryFirstOrDefaultAsync<long>(CountTagPostCommand, new { tag });

                            await service.Connection.ExecuteAsync(UpdateSmartLookupTagCommand, new
                            {
                                value = countTagPost,
                                tag,
                                KeywordType = (int)payload.KeywordType
                            });
                        }
                    }
                    break;
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

    #region -- Properties --

    private string UpdateSmartLookupPeopleCommand
    {
        get
        {
            return string.Format(@"UPDATE {0} AS s
                                    SET ""CountCriteria"" = c.count_value
                                    FROM (
                                        SELECT u.""ProfileName"", COUNT(p.""Id"") AS count_value
                                        FROM {1} p
                                        INNER JOIN {2} u ON p.""UserId"" = u.""Id""
                                        WHERE u.""ProfileName"" = @Name AND p.""IsDelete"" = false
                                        GROUP BY u.""ProfileName""
                                    ) AS c
                                    WHERE s.""Keyword"" = c.""ProfileName""
                                    AND s.""Keyword"" = @Name
                                    AND s.""KeywordType"" = @KeywordType;
                                ", "public.\"SmartLookups\"", "public.\"Posts\"", "identity.\"Users\"");
        }
    }

    private string UpdateSmartLookupTagCommand
    {
        get
        {
            return string.Format(@"UPDATE {0} AS s
                                        SET ""CountCriteria"" = @value
                                        WHERE s.""Keyword"" = @tag
                                        AND s.""KeywordType"" = @KeywordType;"
                , "public.\"SmartLookups\"");
        }
    }

    private string CountTagPostCommand
    {
        get
        {
            return @$"SELECT COUNT(tagpost.""PostId"") AS CountValue
                        FROM {"public.\"Tags\""} tag 
                        INNER JOIN {"public.\"TagPosts\""} tagpost ON tagpost.""TagId"" = tag.""Id""
                        WHERE tag.""Name"" = @tag AND tagpost.""IsDelete"" = false";
        }
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
