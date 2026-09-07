namespace Mcsg.Api.Areas.Story.Models;

using Common.Core.Distributor;
using Common.Models;

public class SyncDataDistributeItem : DistributedItem
{
    public SyncData Data { get; set; }
}
