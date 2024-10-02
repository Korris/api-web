namespace Mcsg.Wallet.Api.Requests;

using Common.Core.Requests;

public class UserWalletTransactionSearchR : PagingR
{
    public DateTime? ToDate { get; set; }
    public DateTime? FromDate { get; set; }
    public List<string>? TransactionType { get; set; } = [];
    public List<string>? TransactionStatus { get; set; } = [];
    public string? ReferenceNumber { get; set; }
}
