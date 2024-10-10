namespace Mcsg.Realtime.Api.Requests;

using Wallet.Domain.Enums;

public class TransactionNotificationReq
{
    public Guid Id { get; set; }
    public Guid AuthorId { get; set; }
    public Guid ReceiverId { get; set; }
    public TransactionType TransactionType { get; set; }
    public float Amount { get; set; }
    public string? ReferenceNumber { get; set; }
}
