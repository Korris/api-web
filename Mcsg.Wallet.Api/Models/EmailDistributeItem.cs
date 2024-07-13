namespace Mcsg.Wallet.Api.Models;

using Common.Core.Distributor;
using Common.Core.Enums;
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
