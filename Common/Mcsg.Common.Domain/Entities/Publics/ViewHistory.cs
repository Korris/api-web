using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

using SeedWork;
using Core.Enums;

[Table("ViewHistories")]
public class ViewHistory : EntityId
{
    public EntityType EntityType { get; set; }
    public EntitySubType? SubType { get; set; }
    public string? IpAddress { get; set; }
    public Guid EntityId { get; set; }
    public Guid UsedId { get; set; }
    public DateTime? CreatedOn { get; set; }
}
