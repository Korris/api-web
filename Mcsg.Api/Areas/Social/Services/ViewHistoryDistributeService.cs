namespace Mcsg.Api.Areas.Social.Services;

using Common.Core.Distributor;
using Common.Core.Dtos;
using Common.Core.Extensions;
using Mcsg.Api.Areas.Social.Interfaces;
using Mcsg.Api.Areas.Social.Models;
using Mcsg.Api.Interfaces;

public class ViewHistoryDistributeService : BaseDistributor
{
    public ViewHistoryDistributeService(IServiceProvider serviceProvider)
    {
        _setting = serviceProvider.GetRequiredService<ISetting>();
    }

    public override Task<bool> IsAcceptable(DistributedItem item)
    {
        var isAcceptable = item.GetType() == typeof(ViewHistoryDistributeItem);
        return Task.FromResult(isAcceptable);
    }

    public override async Task ApplyAction(DistributedItem item)
    {
        var distributeItem = item as ViewHistoryDistributeItem;
        var msg = new QueueMessageDto(distributeItem.Data);
        _setting.SendMessageToQueue(_setting.NotificationExchange, _setting.NotificationQueueViewHistory, msg);
    }

    #region -- Fields --

    /// <summary>
    /// Setting
    /// </summary>
    private readonly ISetting _setting;

    #endregion
}
