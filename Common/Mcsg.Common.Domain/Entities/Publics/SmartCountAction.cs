using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

using SeedWork;
using Core.Enums;

[Table("SmartCountActions")]
public class SmartCountAction : BaseEntity
{
    public EntityType EntityType { get; set; }
    public EntitySubType? SubType { get; set; }
    public DateOnly Date { get; set; }
    public Guid EntityId { get; set; }
    public ActionType ActionType { get; set; }
    public int Count { get; set; }
    public DateTime? ModifiedDate { get; set; }
}
