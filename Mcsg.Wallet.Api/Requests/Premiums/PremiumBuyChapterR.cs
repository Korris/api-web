using System.ComponentModel;

namespace Mcsg.Wallet.Api.Requests;

using Constants;

public class PremiumBuyChapterR
{
    public Guid ChapterId { get; set; }
    [DefaultValue(PayMethods.POINT)]
    public string PayMethodName { get; set; }
    public string AffiliateCode { get; set; }
}
