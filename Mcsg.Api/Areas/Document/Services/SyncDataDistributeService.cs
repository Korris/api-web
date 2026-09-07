using Mcsg.Api.Interfaces;
﻿namespace Mcsg.Api.Areas.Document.Services;

using Common.Core.Distributor;
using Common.Core.Dtos;
using Common.Core.Extensions;
using Mcsg.Api.Areas.Document.Interfaces;
using Mcsg.Api.Areas.Document.Models;

public class SyncDataDistributeService : BaseDistributor
{
    public SyncDataDistributeService(IServiceProvider serviceProvider)
    {
        _setting = serviceProvider.GetRequiredService<ISetting>();
    }

    public override Task<bool> IsAcceptable(DistributedItem item)
    {
        var isAcceptable = item.GetType() == typeof(SyncDataDistributeItem);
        return Task.FromResult(isAcceptable);
    }

    public override async Task ApplyAction(DistributedItem item)
    {
        var distributeItem = item as SyncDataDistributeItem;
        var msg = new QueueMessageDto(distributeItem.Data);
        _setting.SendMessageToQueue(_setting.NotificationExchange, _setting.NotificationQueueSyncData, msg);
    }

    #region -- Fields --

    /// <summary>
    /// Setting
    /// </summary>
    private readonly ISetting _setting;

    #endregion
}
