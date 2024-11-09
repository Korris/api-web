namespace Mcsg.Identity.Api.Response;

using Common.Core.Distributor;
using Common.Models;

public class SyncDataDistributeItem : DistributedItem
{
    public SyncData Data { get; set; }
}
