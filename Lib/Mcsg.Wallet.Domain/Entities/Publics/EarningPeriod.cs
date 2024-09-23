using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Wallet.Domain.Entities;

using Common.SeedWork;

public partial class EarningPeriod : AuditableEntity
{
    [MaxLength(50)]
    public string? Title { get; set; } // 2024-001 => Year - number of payment
    public int Year { get; set; }
    public int Order { get; set; } // Manual

    [Column(TypeName = "timestamp")]
    public DateTime FromDate { get; set; }

    [Column(TypeName = "timestamp")]
    public DateTime ToDate { get; set; }
}
