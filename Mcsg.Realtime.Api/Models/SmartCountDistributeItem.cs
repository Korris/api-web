using Mcsg.Lib.Common.Distributor;
using Mcsg.Lib.Common.Models;

namespace Mcsg.Realtime.Api.Models
{
    public class SmartCountDistributeItem : DistributedItem
    {
        public SmartCountEntityData Data { get; set; }
    }
}
