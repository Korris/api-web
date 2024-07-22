using System.ComponentModel.DataAnnotations;

namespace Mcsg.Lib.Data.Wallet.Entities
{
    using Enums;
    using Mcsg.Common.SeedWork;

    public class WalletTransaction : AuditableEntity
    {
        [MaxLength(32)]
        public string? ReferenceNumber { get; set; }
        public Guid? SourceUserWalletId { get; set; }
        public virtual UserWallet SourceUserWallet { get; set; }

        public Guid? DestinationUserWalletId { get; set; }
        public virtual UserWallet DestinationUserWallet { get; set; }
        public Guid? UserPaymentMethodId { get; set; }
        public virtual UserPaymentMethod UserPaymentMethods { get; set; }
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

        public virtual ICollection<WalletTransactionOtp> WalletTransactionOtps { get; set; }
        public virtual ICollection<UserPurchaseTransaction> UserPurchaseTransactions { get; set; }

    }
}
