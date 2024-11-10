using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

using Core.Enums;
using SeedWork;

public partial class ViewHistory : EntityId
{
    public EntityType EntityType { get; set; }
    public EntitySubType? SubType { get; set; }
    public string? IpAddress { get; set; }
    public Guid EntityId { get; set; }
    public Guid UsedId { get; set; }

    [Column(TypeName = "timestamp")]
    public DateTime? CreatedOn { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("ViewHistories")]
    public virtual User? User { get; set; }
}
