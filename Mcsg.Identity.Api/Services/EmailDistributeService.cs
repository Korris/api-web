using Newtonsoft.Json;

namespace Mcsg.Identity.Api.Services
{
    using Common.Core.Dtos;
    using Common.Core.Extensions;
    using Interfaces;
    using Lib.Common.Distributor;
    using Lib.Data.Domain.Entities;
    using Lib.Data.Enums;
    using Lib.Data.Repositories;
    using Lib.Data.Repositories.Interface;
    using Models;

    public class EmailDistributeService : BaseDistributor
    {
        public EmailDistributeService(IServiceProvider serviceProvider)
        {
            _jobRepository = serviceProvider.GetRequiredService<IUnitOfWork>().GetRepository<Job>();
            _setting = serviceProvider.GetRequiredService<ISetting>();
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

            var msg = new QueueMessageDto
            {
                DevName = emailItem.Id.ToString()
            };
            _setting.SendMessageToQueue(_setting.NotificationExchange, _setting.NotificationQueueEmail, msg);
        }

        #region -- Fields --

        /// <summary>
        /// Job repository
        /// </summary>
        private readonly IRepository<Job> _jobRepository;

        /// <summary>
        /// Setting
        /// </summary>
        private readonly ISetting _setting;

        #endregion
    }
}
