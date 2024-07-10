namespace Mcsg.Identity.Api.Services;

using Common.Core.Dtos;
using Common.Core.Enums;
using Common.Core.Extensions;
using Interfaces;
using Lib.Common.Distributor;
using Lib.Common.Extensions;
using Lib.Data.Domain.Entities;
using Lib.Data.Repositories;
using Lib.Data.Repositories.Interface;
using Models;

public class SmsDistributeService : BaseDistributor
{
    public SmsDistributeService(IServiceProvider serviceProvider)
    {
        _jobRepository = serviceProvider.GetRequiredService<IUnitOfWork>().GetRepository<Job>();
        _setting = serviceProvider.GetRequiredService<ISetting>();
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

        var msg = new QueueMessageDto
        {
            DevName = smsItem.Id.ToString()
        };
        _setting.SendMessageToQueue(_setting.NotificationExchange, _setting.NotificationQueueSms, msg);
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
