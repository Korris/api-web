namespace Mcsg.Api.Areas.Realtime.Dtos;

using Common.Core.Distributor;
using Common.Models;

public class SmartCountDistributeDto : DistributedItem
{
    public SmartCountEntityData Data { get; set; }
}
