using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

using Common;
using Core.Enums;

[Table("ViewHistories")]
public class ViewHistory : BaseEntity
{
    public EntityType EntityType { get; set; }
    public EntitySubType? SubType { get; set; }
    public string? IpAddress { get; set; }
    public Guid EntityId { get; set; }
    public Guid UsedId { get; set; }
    public DateTime? CreatedDate { get; set; }
}
