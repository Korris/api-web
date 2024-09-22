using System.ComponentModel.DataAnnotations;

namespace Mcsg.Wallet.Domain.Entities;

using Mcsg.Common.Core.Enums;
using Mcsg.Common.SeedWork;

public class PaymentMethod : AuditableEntity
{
    [MaxLength(255)]
    public string? Name { get; set; }
    [MaxLength(255)]
    public string? Logo { get; set; }
    public bool AllowDeposit { get; set; }
    public bool AllowWithdrawal { get; set; }
    public int SelfId { get; set; }
    public string? Code { get; set; }
    public string? Bin { get; set; }
    public string? ShortName { get; set; }
    public string? SwiftCode { get; set; }
    public bool IsActive { get; set; }
    public PaymentMethodType Type { get; set; }
    public virtual ICollection<UserPaymentMethod> UserPaymentMethods { get; set; }
}
