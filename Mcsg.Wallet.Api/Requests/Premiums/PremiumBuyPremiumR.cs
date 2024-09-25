using System.ComponentModel;

namespace Mcsg.Wallet.Api.Requests;

using Common.Core.Requests;
using Constants;

public class PremiumBuyPremiumR : BaseR
{
    public int PremiumPackageNo { get; set; }
    [DefaultValue(PayMethods.POINT)]
    public string? PayMethodName { get; set; }
    public string? AffiliateCode { get; set; }
}
