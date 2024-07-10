namespace Mcsg.Identity.Api.Models;

using Common.Core.Enums;
using Lib.Common.Distributor;
using Lib.Common.Models;

public class SmsJobDistributeItem : DistributedItem
{
    public SmsJobDistributeItem()
    {
        Id = Guid.NewGuid();
        Sms = new Sms();
    }

    public Sms Sms { get; set; }
    public JobType JobType { get; set; }
}
