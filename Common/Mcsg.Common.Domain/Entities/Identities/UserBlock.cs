using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

using Core.Enums;
using SeedWork;

public class UserBlock : AuditableEntity
{
    #region -- Properties --

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

    #endregion
}