using Mcsg.Admin.Api.DTOs.PaymentMenthods;
using Mcsg.Lib.Data.Wallet.Enums;

namespace Mcsg.Admin.Api.DTOs.Transactions
{
    public class UserWalletTransactionItemResp
    {
        public Guid Id { get; set; }
        public string ReferenceNumber { get; set; }
        public string FromAddress { get; set; }
        public string FromUser { get; set; }
        public string ToAddress { get; set; }
        public string ToUser { get; set; }
        public float Amount { get; set; }
        public string AmountSign { get; set; }
        public TransactionType TransactionType { get; set; }
        public TransactionStatus TransactionStatus { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Content { get; set; }
        public string SystemMessage { get; set; }
    }
    public class UserWalletTransactionItemDetailResp : UserWalletTransactionItemResp
    {
        public Guid? FromUserId { get; set; }
        public Guid? ToUserId { get; set; }
        public Guid? PaymentMethodId { get; set; }
        public PaymentMethodResp? PaymentMethod { get; set; }
    }
    public class UserPurchaseTransactionItemDetailResp : UserWalletTransactionItemResp
    {
        public string Title { get; set; }
        public string Thumbnail { get; set; }
        public string Paymethod { get; set; }
    }
}
