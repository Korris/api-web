namespace Mcsg.Realtime.Api.Dtos;

using Common.Core.Distributor;
using Lib.Common.Models;

public class SmartCountDistributeDto : DistributedItem
{
    public SmartCountEntityData Data { get; set; }
}
