namespace Mcsg.Document.Api.Models;

using Common.Core.Distributor;
using Lib.Common.Models;

public class SyncDataDistributeItem : DistributedItem
{
    public SyncData Data { get; set; }
}
