namespace Mcsg.Wallet.Api.Models;

using Lib.Common.Distributor;
using Lib.Common.Models;

public class SyncDataDistributeItem : DistributedItem
{
    public SyncData Data { get; set; }
}
