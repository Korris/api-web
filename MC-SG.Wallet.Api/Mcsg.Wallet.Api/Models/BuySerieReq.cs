using Mcsg.Wallet.Api.Constants;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Mcsg.Wallet.Api.Models
{
    public class BuySerieReq
    {
        [Required]
        public Guid SerieId { get; set; }
        [DefaultValue(PayMethods.POINT)]
        public string PayMethodName { get; set; }
        public string AffiliateCode { get; set; }
    }
}
