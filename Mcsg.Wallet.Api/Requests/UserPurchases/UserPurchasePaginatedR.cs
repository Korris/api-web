using System.ComponentModel;

namespace Mcsg.Wallet.Api.Requests;

public class UserPurchasePaginatedR
{
    [DefaultValue(1)]
    public int PageNumber { get; set; }
    [DefaultValue(10)]
    public int PageSize { get; set; }
    [DefaultValue("CreatedDate")]
    public string OrderBy { get; set; }
}
