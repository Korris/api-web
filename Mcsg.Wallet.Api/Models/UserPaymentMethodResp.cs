using Mcsg.Lib.Data.Wallet.Enums;

namespace Mcsg.Wallet.Api.Models
{
    public class UserPaymentMethodResp
    {
        public Guid Id { get; set; }
        public PaymentMethodType PaymentMethodType { get; set; }
        public Guid PaymentMethodId { get; set; }
        public string PaymentMethodName { get; set; }
    }
}
