namespace Mcsg.Wallet.Api.Models
{
    public class UserPaymentMethodReq
    {
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
    }
    public class AddUserPaymentMethodReq : UserPaymentMethodReq
    {
        public Guid PaymentMethodId { get; set; }
    }
    public class UpdateUserPaymentMethodReq : UserPaymentMethodReq
    {
        public Guid? PaymentMethodId { get; set; }
    }
}
