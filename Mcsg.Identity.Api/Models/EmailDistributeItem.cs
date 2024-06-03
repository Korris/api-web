using Mcsg.Lib.Common.Distributor;
using Mcsg.Lib.Common.Models;
using Mcsg.Lib.Data.Enums;

namespace Mcsg.Identity.Api.Models
{
    public class EmailJobDistributeItem : DistributedItem
    {
        public EmailJobDistributeItem()
        {
            Id = Guid.NewGuid();
            Email = new Email();
        }

        public Email Email { get; set; }
        public JobType JobType { get; set; }
    }
}
