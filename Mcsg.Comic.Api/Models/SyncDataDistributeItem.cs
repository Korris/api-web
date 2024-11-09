namespace Mcsg.Comic.Api.Models;

using Common.Core.Distributor;
using Common.Models;

public class SyncDataDistributeItem : DistributedItem
{
    public SyncData Data { get; set; }
}
