using Mcsg.Function.Job.Services;
using Mcsg.Lib.Common.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Mcsg.Function.Job
{
    public class SyncDataFunction
    {
        private readonly ILogger<SyncDataFunction> _logger;
        private readonly ISyncDataService _syncDataService;
        public SyncDataFunction(ILogger<SyncDataFunction> logger, ISyncDataService syncDataService)
        {
            _syncDataService = syncDataService;
            _logger = logger;
        }

        [Function(nameof(SyncDataFunction))]
        public async Task Run([QueueTrigger("syncdataqueue", Connection = "Function:AzureBlobStorageConnection")] string data)
        {
            string logMessage = $"C# Queue trigger function processed: {data}";
            _logger.LogInformation(logMessage);
            var syncData = JsonConvert.DeserializeObject<SyncData>(data);

            switch (syncData.TargetDb)
            {
                case SyncTargetDb.WALLETDB:
                    {
                        if (syncData.TargetEntity == SyncTargetEntity.WALLET_USER_INFO)
                        {
                            await _syncDataService.SyncWalletUserInfoAsync(syncData);
                        }
                        if (syncData.TargetEntity == SyncTargetEntity.WALLET_USER_REWARD)
                        {
                            await _syncDataService.SyncWalletUserRewardAsync(syncData);
                        }
                        if (syncData.TargetEntity == SyncTargetEntity.WALLET_USER_BUY_PREMIUM)
                        {
                            await _syncDataService.SyncUserPremiumAsync(syncData);
                        }
                        if (syncData.TargetEntity == SyncTargetEntity.WALLET_USER_BUY_CHAPTER)
                        {
                            await _syncDataService.SyncUserBuyChapterAsync(syncData);
                        }
                        if (syncData.TargetEntity == SyncTargetEntity.WALLET_USER_BUY_SERIES)
                        {
                            await _syncDataService.SyncUserBuySeriesAsync(syncData);
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
}
