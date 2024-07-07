namespace Mcsg.Social.Api.Models;

using Lib.Common.Distributor;
using Lib.Common.Models;

public class ViewHistoryDistributeItem : DistributedItem
{
    public ViewHistoryData Data { get; set; }
}
