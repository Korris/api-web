using Mcsg.Wallet.Api.Constants;

namespace Mcsg.Wallet.Api.Models
{
    public class CurrencyTypeRatio
    {
        public CurrencyType Type { get; set; }
        public float Ratio { get; set; }
    }
    public class WithDrawPrepareResp
    {
        public List<UserPaymentMethodResp> UserPaymentMethods { get; set; }
        public List<CurrencyTypeRatio> CurrencyTypes { get; set; }
        public float MinPointCanWithDraw { get; set; }
        public float MaxPointCanWithDraw { get; set; }

    }
}
