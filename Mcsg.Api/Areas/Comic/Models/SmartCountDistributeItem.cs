namespace Mcsg.Api.Areas.Comic.Models;

using Common.Core.Distributor;
using Common.Models;

public class SmartCountDistributeItem : DistributedItem
{
    public SmartCountEntityData Data { get; set; }
}
