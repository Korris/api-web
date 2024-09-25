using Newtonsoft.Json;

namespace Mcsg.Wallet.Api.Services;

using Common.Core.Distributor;
using Common.Core.Dtos;
using Common.Core.Enums;
using Common.Core.Extensions;
using Common.Domain.Entities;
using Interfaces;
using Models;

public class EmailDistributeService : BaseDistributor
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="serviceProvider"></param>
    public EmailDistributeService(IServiceProvider serviceProvider)
    {
        _setting = serviceProvider.GetRequiredService<ISetting>();
    }

    public override Task<bool> IsAcceptable(DistributedItem item)
    {
        var isAcceptable = item.GetType() == typeof(EmailJobDistributeItem);
        return Task.FromResult(isAcceptable);
    }

    public override async Task ApplyAction(DistributedItem item)
    {
        var dItem = item as EmailJobDistributeItem;
        var job = new Job
        {
            Id = dItem.Id,
            JobType = dItem.JobType,
            Data = JsonConvert.SerializeObject(dItem.Email),
            Status = JobStatus.Queued
        };

        var msg = new QueueMessageDto(job)
        {
            DevName = dItem.Id.ToString()
        };
        _setting.SendMessageToQueue(_setting.NotificationExchange, _setting.NotificationQueueEmail, msg);
    }

    #endregion

    #region -- Fields --

    /// <summary>
    /// Setting
    /// </summary>
    private readonly ISetting _setting;

    #endregion
}
