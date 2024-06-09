using Mcsg.Social.Api.Models;
using Mcsg.Lib.AzureBlobStorage;
using Mcsg.Lib.Common.Distributor;
using Newtonsoft.Json;

namespace Mcsg.Social.Api.Services
{
    public class ViewHistoryDistributeService : BaseDistributor
    {
        private readonly IAzureBlobStorageQueueService _queueService;

        public ViewHistoryDistributeService(IServiceProvider serviceProvider)
        {
            _queueService = serviceProvider.GetRequiredService<IAzureBlobStorageQueueService>();
        }

        public override Task<bool> IsAcceptable(DistributedItem item)
        {
            var isAcceptable = item.GetType() == typeof(ViewHistoryDistributeItem);
            return Task.FromResult(isAcceptable);
        }

        public override async Task ApplyAction(DistributedItem item)
        {
            var distributeItem = item as ViewHistoryDistributeItem;
            var queueData = JsonConvert.SerializeObject(distributeItem.Data);
            await _queueService.Enqueue("viewhistoryqueue", queueData);
        }
    }
}
