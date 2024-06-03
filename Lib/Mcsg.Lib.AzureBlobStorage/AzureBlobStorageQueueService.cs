using Azure.Storage.Queues;
using Mcsg.Lib.AzureBlobStorage.Settings;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Mcsg.Lib.AzureBlobStorage
{
    public interface IAzureBlobStorageQueueService
    {
        QueueClient GetQueueClient(string queueName);
        Task CreateQueueIfNotExist(string queueName);
        Task Enqueue(string queueName, string message);
        Task EnqueueAsJson(string queueName, object message);
    }

    public class AzureBlobStorageQueueService : IAzureBlobStorageQueueService
    {
        private readonly AzureBlobStorageSettings _settings;
        private readonly string _connectionString;

        public AzureBlobStorageQueueService(IOptions<AzureBlobStorageSettings> settings)
        {
            _settings = settings.Value;
        }
        public AzureBlobStorageQueueService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public AzureBlobStorageQueueService(AzureBlobStorageSettings settings)
        {
            _settings = settings;
        }

        public QueueClient GetQueueClient(string queueName)
        {
            var option = new QueueClientOptions
            {
                MessageEncoding = QueueMessageEncoding.Base64
            };

            if (!string.IsNullOrEmpty(_connectionString))
            {
                return new QueueClient(_connectionString, queueName, option);
            }
            else
            {
                return new QueueClient(string.Format(LibConstant.CONNECTION_STRING, _settings.StorageName, _settings.AccountKey), queueName, option);
            }

        }

        public async Task CreateQueueIfNotExist(string queueName)
        {
            var client = GetQueueClient(queueName);
            await client.CreateIfNotExistsAsync();
        }

        public async Task Enqueue(string queueName, string message)
        {
            var client = GetQueueClient(queueName);
            await client.SendMessageAsync(message);
        }

        public async Task EnqueueAsJson(string queueName, object message)
        {
            var json = JsonConvert.SerializeObject(message);
            var client = GetQueueClient(queueName);
            await client.SendMessageAsync(json);
        }
    }
}
