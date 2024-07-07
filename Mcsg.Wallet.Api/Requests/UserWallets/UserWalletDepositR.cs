namespace Mcsg.Wallet.Api.Requests;

using Constants;

public class UserWalletDepositR
{
    public int PointAmount { get; set; }
    public string DepositMethodName { get; set; }
    public CurrencyType CurrencyType { get; set; }
    public string RedirectUrl { get; set; }
}
