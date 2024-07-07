namespace Mcsg.Wallet.Api.Models;

public class DepositPrepareResp
{
    public List<PayMethodResp> DepositMethods { get; set; }
    public List<CurrencyTypeRatio> CurrencyTypes { get; set; }
    public float MinPointCanDeposit { get; set; }
    public float MaxPointCanDeposit { get; set; }

}
