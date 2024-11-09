namespace Mcsg.Realtime.Api.Dtos;

using Common.Core.Distributor;
using Common.Models;

public class SmartCountDistributeDto : DistributedItem
{
    public SmartCountEntityData Data { get; set; }
}
