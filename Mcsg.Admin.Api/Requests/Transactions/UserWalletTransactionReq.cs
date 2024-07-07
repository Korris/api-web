namespace Mcsg.Admin.Api.Requests
{
    using Lib.Data.Wallet.Enums;

    public class UserWalletTransactionReq : GetByPageR
    {
        public List<TransactionType>? Types { get; set; }
        public TransactionStatus? Status { get; set; }
        public Guid? UserId { get; set; }
        public Guid? TransactionId { get; set; }
        public string? ReferenceNumber { get; set; }
    }
}
