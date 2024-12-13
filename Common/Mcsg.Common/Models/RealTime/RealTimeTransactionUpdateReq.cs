namespace Mcsg.Common.Models.RealTime;

using Core.Enums;

public class RealTimeTransactionUpdateReq
{
    public Guid UserId { get; set; }
    public PaymentMethodType PaymentType { get; set; }
    public string TransactionId { get; set; } = string.Empty;
    public string ReferenceNumber { get; set; } = string.Empty;
    public TransactionStatus TransactionStatus { get; set; }
    public string WalletAddress { get; set; }
    public decimal Point { get; set; }
    public decimal RewardPoint { get; set; }
    public decimal TotalPoint { get; set; }
}
