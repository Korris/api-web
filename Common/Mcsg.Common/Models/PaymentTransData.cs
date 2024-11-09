namespace Mcsg.Lib.Common.Models;

using Mcsg.Common.Core.Enums;

public class PaymentTransData
{
    public PaymentTransType Type { get; set; }
    public Guid TransactionId { get; set; }
    public Guid UserId { get; set; }
    public TransactionStatus Status { get; set; }
}
public enum PaymentTransType
{
    BANK,
    ZALO_PAY,
    MOMO
}
