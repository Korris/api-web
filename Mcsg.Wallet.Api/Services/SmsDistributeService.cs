namespace Mcsg.Wallet.Api.Services;

using Common.Core.Distributor;
using Common.Core.Dtos;
using Common.Core.Enums;
using Common.Core.Extensions;
using Domain.Entities;
using Interfaces;
using Lib.Common.Extensions;
using Models;

public class SmsDistributeService : BaseDistributor
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="serviceProvider"></param>
    public SmsDistributeService(IServiceProvider serviceProvider)
    {
        _setting = serviceProvider.GetRequiredService<ISetting>();
    }

    public override Task<bool> IsAcceptable(DistributedItem item)
    {
        var isAcceptable = item.GetType() == typeof(SmsJobDistributeItem);
        return Task.FromResult(isAcceptable);
    }

    public override async Task ApplyAction(DistributedItem item)
    {
        var dItem = item as SmsJobDistributeItem;
        var job = new Job
        {
            Id = dItem.Id,
            JobType = dItem.JobType,
            Data = dItem.Sms.ToJson(),
            Status = JobStatus.Queued
        };

        var msg = new QueueMessageDto(job)
        {
            DevName = dItem.Id.ToString()
        };
        _setting.SendMessageToQueue(_setting.NotificationExchange, _setting.NotificationQueueSms, msg);
    }

    #endregion

    #region -- Fields --

    /// <summary>
    /// Setting
    /// </summary>
    private readonly ISetting _setting;

    #endregion
}
