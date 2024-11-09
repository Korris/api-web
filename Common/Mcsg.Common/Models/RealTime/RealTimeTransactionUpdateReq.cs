namespace Mcsg.Lib.Common.Models.RealTime;

using Mcsg.Common.Core.Enums;

public class RealTimeTransactionUpdateReq
{
    public Guid UserId { get; set; }
    public PaymentMethodType PaymentType { get; set; }
    public string TransactionId { get; set; } = string.Empty;
    public string ReferenceNumber { get; set; } = string.Empty;
    public TransactionStatus TransactionStatus { get; set; }
    public string WalletAddress { get; set; }
    public float Point { get; set; }
    public float RewardPoint { get; set; }
    public float TotalPoint { get; set; }
}
