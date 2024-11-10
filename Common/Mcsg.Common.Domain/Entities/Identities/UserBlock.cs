using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

using Core.Enums;
using SeedWork;

public partial class UserBlock : AuditableEntity
{
    /// <summary>
    /// UserId1
    /// </summary>
    public Guid UserId1 { get; set; }

    /// <summary>
    /// UserId2
    /// </summary>
    public Guid UserId2 { get; set; }

    /// <summary>
    /// Type
    /// </summary>
    public BlockType Type { get; set; }

    /// <summary>
    /// Reason
    /// </summary>
    public string? Reason { get; set; }

    /// <summary>
    /// Start date
    /// </summary>
    [Column(TypeName = "timestamp")]
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// End date
    /// </summary>
    [Column(TypeName = "timestamp")]
    public DateTime? EndDate { get; set; }

    [ForeignKey("UserId1")]
    [InverseProperty("UserBlockUserId1Navigations")]
    public virtual User UserId1Navigation { get; set; } = null!;

    [ForeignKey("UserId2")]
    [InverseProperty("UserBlockUserId2Navigations")]
    public virtual User UserId2Navigation { get; set; } = null!;
}
