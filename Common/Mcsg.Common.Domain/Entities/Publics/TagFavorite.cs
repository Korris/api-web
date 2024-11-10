using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

using SeedWork;

public partial class TagFavorite : AuditableEntity
{
    public Guid TagId { get; set; }
    public Guid UserId { get; set; }

    [ForeignKey("TagId")]
    [InverseProperty("TagFavorites")]
    public virtual Tag Tag { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("TagFavorites")]
    public virtual User User { get; set; } = null!;
}
