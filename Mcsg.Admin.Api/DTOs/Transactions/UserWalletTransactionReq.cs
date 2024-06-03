using Mcsg.Lib.Data.Wallet.Enums;

namespace Mcsg.Admin.Api.DTOs.Transactions
{
    public class UserWalletTransactionReq : GetByPageReq
    {
        public List<TransactionType>? Types { get; set; }
        public TransactionStatus? Status { get; set; }
        public Guid? UserId { get; set; }
        public Guid? TransactionId { get; set; }
        public string? ReferenceNumber { get; set; }
    }
}
