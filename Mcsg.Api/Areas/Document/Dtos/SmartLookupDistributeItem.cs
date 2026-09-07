namespace Mcsg.Api.Areas.Document.Dtos;

using Common.Core.Distributor;
using Common.Core.Dtos;

public class SmartLookupDistributeItem : DistributedItem
{
    public SmartLookupDto Data { get; set; }
}
