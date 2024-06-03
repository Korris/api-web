using Mcsg.Wallet.Api.Constants;

namespace Mcsg.Wallet.Api.Models
{
    public class WithdrawReq
    {
        public float Amount { get; set; }
        public string Content { get; set; }
        public CurrencyType Type { get; set; }
        public Guid UserPaymentMethodId { get; set; }
    }
}
