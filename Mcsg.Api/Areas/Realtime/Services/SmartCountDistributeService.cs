namespace Mcsg.Api.Areas.Realtime.Services;

using Common.Core.Distributor;
using Common.Core.Dtos;
using Common.Core.Extensions;
using Mcsg.Api.Areas.Realtime.Dtos;
using Mcsg.Api.Interfaces;

public class SmartCountDistributeService : BaseDistributor
{
    public SmartCountDistributeService(IServiceProvider serviceProvider)
    {
        _setting = serviceProvider.GetRequiredService<ISetting>();
    }

    public override Task<bool> IsAcceptable(DistributedItem item)
    {
        var isAcceptable = item.GetType() == typeof(SmartCountDistributeDto);
        return Task.FromResult(isAcceptable);
    }

    public override async Task ApplyAction(DistributedItem item)
    {
        var distributeItem = item as SmartCountDistributeDto;
        var msg = new QueueMessageDto(distributeItem.Data);
        _setting.SendMessageToQueue(_setting.NotificationExchange, _setting.NotificationQueuePostComment, msg);
    }

    #region -- Fields --

    private readonly ISetting _setting;

    #endregion
}
