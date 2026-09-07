namespace Mcsg.Api.Areas.Story.Services;

using Mcsg.Api.Interfaces;
using Common.Core.Distributor;
using Common.Core.Dtos;
using Common.Core.Extensions;
using Mcsg.Api.Areas.Story.Interfaces;
using Mcsg.Api.Areas.Story.Models;

public class SmartCountDistributeService : BaseDistributor
{
    public SmartCountDistributeService(IServiceProvider serviceProvider)
    {
        _setting = serviceProvider.GetRequiredService<ISetting>();
    }

    public override Task<bool> IsAcceptable(DistributedItem item)
    {
        var isAcceptable = item.GetType() == typeof(SmartCountDistributeItem);
        return Task.FromResult(isAcceptable);
    }

    public override async Task ApplyAction(DistributedItem item)
    {
        var distributeItem = item as SmartCountDistributeItem;
        var msg = new QueueMessageDto(distributeItem.Data);
        _setting.SendMessageToQueue(_setting.NotificationExchange, _setting.NotificationQueuePostReact, msg);
    }

    #region -- Fields --

    /// <summary>
    /// Setting
    /// </summary>
    private readonly ISetting _setting;

    #endregion
}
