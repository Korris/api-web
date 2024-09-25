namespace Mcsg.Wallet.Api.Requests;

using Common.Core.Requests;
using Constants;

public class UserWalletDepositR : BaseR
{
    public int PointAmount { get; set; }
    public string DepositMethodName { get; set; }
    public CurrencyType CurrencyType { get; set; }
    public string RedirectUrl { get; set; }
}
