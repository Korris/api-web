using Mcsg.Lib.AzureBlobStorage;
using Mcsg.Lib.Common.Distributor;
using Mcsg.Realtime.Api.Models;
using Newtonsoft.Json;

namespace Mcsg.Realtime.Api.Services
{
    public class SmartCountDistributeService : BaseDistributor
    {
        private readonly IAzureBlobStorageQueueService _queueService;

        public SmartCountDistributeService(IServiceProvider serviceProvider)
        {
            _queueService = serviceProvider.GetRequiredService<IAzureBlobStorageQueueService>();
        }

        public override Task<bool> IsAcceptable(DistributedItem item)
        {
            var isAcceptable = item.GetType() == typeof(SmartCountDistributeItem);
            return Task.FromResult(isAcceptable);
        }

        public override async Task ApplyAction(DistributedItem item)
        {
            var distributeItem = item as SmartCountDistributeItem;
            var queueData = JsonConvert.SerializeObject(distributeItem.Data);
            await _queueService.Enqueue("postcommentqueue", queueData);
        }
    }
}
