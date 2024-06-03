namespace Mcsg.Wallet.Api.Models
{
    public class AddUserPaymentMethodResp : AddUserPaymentMethodReq
    {
        public Guid Id { get; set; }
    }
    public class UpdateUserPaymentMethodResp : AddUserPaymentMethodReq
    {
        public Guid Id { get; set; }
    }
}
