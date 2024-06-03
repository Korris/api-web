using Mcsg.Wallet.Api.Constants;
using System.ComponentModel;

namespace Mcsg.Wallet.Api.Models
{
    public class BuyChapterReq
    {
        public Guid ChapterId { get; set; }
        [DefaultValue(PayMethods.POINT)]
        public string PayMethodName { get; set; }
        public string AffiliateCode { get; set; }
    }
}
