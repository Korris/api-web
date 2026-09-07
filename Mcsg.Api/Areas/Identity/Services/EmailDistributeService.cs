using Newtonsoft.Json;

namespace Mcsg.Api.Areas.Identity.Services;

using Common.Core.Distributor;
using Common.Core.Dtos;
using Common.Core.Enums;
using Common.Core.Extensions;
using Common.Domain;
using Common.Domain.Entities;
using Mcsg.Api.Areas.Identity.Interfaces;
using Mcsg.Api.Interfaces;
using Models;

public class EmailDistributeService : BaseDistributor
{
    public EmailDistributeService(IServiceProvider serviceProvider)
    {
        _context = serviceProvider.GetRequiredService<IMcsgContext>();
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
        await _context.Jobs.AddAsync(job);
        await _context.SaveChangesAsync(default);

        var msg = new QueueMessageDto
        {
            DevName = emailItem.Id.ToString()
        };

        _setting.SendMessageToQueue(_setting.NotificationExchange, _setting.NotificationQueueEmail, msg);
    }

    #region -- Fields --

    /// <summary>
    /// DB context
    /// </summary>
    private readonly IMcsgContext _context;

    /// <summary>
    /// Setting
    /// </summary>
    private readonly ISetting _setting;

    #endregion
}
