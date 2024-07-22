namespace Mcsg.Wallet.Api.Models;

using Lib.Common.Models;
using Lib.Data.Wallet.Enums;

public class UserWalletTransactionResp
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPage { get; set; }
    public IEnumerable<UserWalletTransactionItemResp> Transactions { get; set; }
}

public class UserWalletTransactionItemResp
{
    public Guid Id { get; set; }
    public string ReferenceNumber { get; set; }
    public string FromAddress { get; set; }
    public string FromUser { get; set; }
    public string ToAddress { get; set; }
    public string ToUser { get; set; }
    public float Amount { get; set; }
    public string AmountOfMoney { get; set; }
    public string AmountSign { get; set; }
    public TransactionType TransactionType { get; set; }
    public TransactionStatus TransactionStatus { get; set; }
    public DateTime CreatedOn { get; set; }
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
public class UserPurchaseOverallResp
{
    public int TotalOrders { get; set; }
    public float TotalAmount { get; set; }
    public string TotalAmountOfMoney { get; set; }
    public PaginatedList<UserPurchaseTransactionItemDetailResp> Orders { get; set; }
}
