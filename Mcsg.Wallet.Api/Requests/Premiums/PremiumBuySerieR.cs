using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Mcsg.Wallet.Api.Requests;

using Constants;

public class PremiumBuySerieR
{
    [Required]
    public Guid SerieId { get; set; }
    [DefaultValue(PayMethods.POINT)]
    public string PayMethodName { get; set; }
    public string AffiliateCode { get; set; }
}
