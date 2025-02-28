namespace Mcsg.Realtime.Api.Requests;

using Common.Core.Enums;

public class TransactionNotificationReq
{
    public Guid Id { get; set; }
    public Guid AuthorId { get; set; }
    public Guid ReceiverId { get; set; }
    public TransactionType TransactionType { get; set; }
    public float Amount { get; set; }
    public string? ReferenceNumber { get; set; }
    public string? CurrencyUnit { get; set; }
    public bool IsUpgradePremium { get; set; }
}
