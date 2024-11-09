namespace Mcsg.Comic.Api.Models;

using Common.Core.Distributor;
using Common.Models;

public class ViewHistoryDistributeItem : DistributedItem
{
    public ViewHistoryData Data { get; set; }
}
