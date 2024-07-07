namespace Mcsg.Wallet.Api.Interfaces;

using Models;

public interface IBankService
{
    Task<IEnumerable<BankResponse>> GetBanks();
    Task<IEnumerable<BankFromApiResp>> SyncBanks();
    List<PayMethodResp> GetDepositMethods();
    List<PayMethodResp> GetPayMethods();
    List<CurrencyTypeRatio> GetCurrencyTypeRatios();
    string GetPayMethodTitle(string name);
}
