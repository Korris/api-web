using Mcsg.Lib.AzureBlobStorage;
using Mcsg.Lib.Common.Distributor;
using Mcsg.Lib.Common.Extensions;
using Mcsg.Lib.Data.Domain.Entities;
using Mcsg.Lib.Data.Enums;
using Mcsg.Lib.Data.Repositories;
using Mcsg.Lib.Data.Repositories.Interface;
using Mcsg.Wallet.Api.Models;

namespace Mcsg.Wallet.Api.Services
{
    public class SmsDistributeService : BaseDistributor
    {
        private readonly IRepository<Job> _jobRepository;
        private readonly IAzureBlobStorageQueueService _queueService;

        public SmsDistributeService(IServiceProvider serviceProvider)
        {
            _jobRepository = serviceProvider.GetRequiredService<IUnitOfWork>().GetRepository<Job>();
            _queueService = serviceProvider.GetRequiredService<IAzureBlobStorageQueueService>();
        }

        public override Task<bool> IsAcceptable(DistributedItem item)
        {
            var isAcceptable = item.GetType() == typeof(SmsJobDistributeItem);
            return Task.FromResult(isAcceptable);
        }

        public override async Task ApplyAction(DistributedItem item)
        {
            var smsItem = item as SmsJobDistributeItem;
            var job = new Job
            {
                Id = smsItem.Id,
                JobType = smsItem.JobType,
                Data = smsItem.Sms.ToJson(),
                Status = JobStatus.Queued
            };
            await _jobRepository.InsertAsync(job);
            await _queueService.Enqueue("smsqueue", $"{smsItem.Id}");
        }
    }
}
