namespace Mcsg.Identity.Api.Models;

using Common.Core.Enums;
using Lib.Common.Distributor;
using Lib.Common.Models;

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
