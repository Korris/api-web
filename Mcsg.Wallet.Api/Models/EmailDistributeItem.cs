namespace Mcsg.Wallet.Api.Models;

using Lib.Common.Distributor;
using Lib.Common.Models;
using Lib.Data.Enums;

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
