namespace Mcsg.Identity.Api.Models;

using Lib.Common.Distributor;
using Lib.Common.Models;
using Lib.Data.Enums;

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
