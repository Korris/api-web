using Mcsg.Lib.Common.Distributor;
using Mcsg.Lib.Common.Models;
using Mcsg.Lib.Data.Enums;

namespace Mcsg.Identity.Api.Models
{
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
}
