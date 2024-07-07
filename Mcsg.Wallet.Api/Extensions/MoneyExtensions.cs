using Microsoft.OpenApi.Extensions;

namespace Mcsg.Wallet.Api.Extensions;

using Constants;

public static class MoneyExtensions
{
    public static string ToMoney(this float point, float ratio = 1, CurrencyType currency = CurrencyType.VND)
    {
        var amount = ratio * point;
        var money = amount.ToString("N0") + " " + currency.GetDisplayName();
        return money;
    }
}
