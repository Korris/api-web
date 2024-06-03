using Mcsg.Wallet.Api.Constants;

namespace Mcsg.Wallet.Api.Models
{
    public class DepositReq
    {
        public int PointAmount { get; set; }
        public string DepositMethodName { get; set; }
        public CurrencyType CurrencyType { get; set; }
        public string RedirectUrl { get; set; }
    }
    public class DepositCancelReq
    {
        public Guid TransactionId { get; set; }
    }
}
