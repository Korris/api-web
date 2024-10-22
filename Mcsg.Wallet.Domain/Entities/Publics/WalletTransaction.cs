using System.ComponentModel.DataAnnotations;

namespace Mcsg.Wallet.Domain.Entities;

using Common.Core.Enums;
using Common.SeedWork;
using Enums;

public partial class WalletTransaction : AuditableEntity
{
    [MaxLength(32)]
    public string? ReferenceNumber { get; set; }
    public Guid? SourceUserWalletId { get; set; }

    public Guid? DestinationUserWalletId { get; set; }
    public Guid? UserPaymentMethodId { get; set; }
    public SystemPaymentMethod? SystemMethod { get; set; }
    public TransactionType Type { get; set; }
    public TransactionStatus Status { get; set; }
    public float Amount { get; set; }

    [MaxLength(255)]
    public string? Content { get; set; }
    public string? SystemMessage { get; set; }
    public bool IsFromSystem { get; set; }

    public bool IsConfirmed { get; set; }
    public Guid? RelatedId { get; set; }
    public string? ExternalId { get; set; }
    public float TransactionFee { get; set; }
}
