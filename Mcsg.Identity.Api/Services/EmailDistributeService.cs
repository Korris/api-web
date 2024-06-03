using Mcsg.Identity.Api.Models;
using Mcsg.Lib.AzureBlobStorage;
using Mcsg.Lib.Common.Distributor;
using Mcsg.Lib.Data.Domain.Entities;
using Mcsg.Lib.Data.Enums;
using Mcsg.Lib.Data.Repositories;
using Mcsg.Lib.Data.Repositories.Interface;
using Newtonsoft.Json;

namespace Mcsg.Identity.Api.Services
{
    public class EmailDistributeService : BaseDistributor
    {
        private readonly IRepository<Job> _jobRepository;
        private readonly IAzureBlobStorageQueueService _queueService;

        public EmailDistributeService(IServiceProvider serviceProvider)
        {
            _jobRepository = serviceProvider.GetRequiredService<IUnitOfWork>().GetRepository<Job>();
            _queueService = serviceProvider.GetRequiredService<IAzureBlobStorageQueueService>();
        }

        public override Task<bool> IsAcceptable(DistributedItem item)
        {
            var isAcceptable = item.GetType() == typeof(EmailJobDistributeItem);
            return Task.FromResult(isAcceptable);
        }

        public override async Task ApplyAction(DistributedItem item)
        {
            var emailItem = item as EmailJobDistributeItem;
            var job = new Job
            {
                Id = emailItem.Id,
                JobType = emailItem.JobType,
                Data = JsonConvert.SerializeObject(emailItem.Email),
                Status = JobStatus.Queued
            };
            await _jobRepository.InsertAsync(job);
            await _queueService.Enqueue("emailqueue", $"{emailItem.Id}");
        }
    }
}
