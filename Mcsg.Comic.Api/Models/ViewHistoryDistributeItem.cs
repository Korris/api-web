namespace Mcsg.Comic.Api.Models;

using Common.Core.Distributor;
using Lib.Common.Models;

public class ViewHistoryDistributeItem : DistributedItem
{
    public ViewHistoryData Data { get; set; }
}
