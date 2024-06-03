using Mcsg.Lib.Common.Distributor;
using Mcsg.Lib.Common.Models;

namespace Mcsg.Wallet.Api.Models
{
    public class SyncDataDistributeItem : DistributedItem
    {
        public SyncData Data { get; set; }
    }
}
