namespace Mcsg.Common.Domain.Entities;

using Core.Enums;
using SeedWork;

public class SmartCountAction : EntityId
{
    public Guid EntityId { get; set; }
    public EntityType EntityType { get; set; }
    public EntitySubType? SubType { get; set; }
    public DateOnly Date { get; set; }
    public ActionType ActionType { get; set; }
    public int Count { get; set; }
    public DateTime? ModifiedOn { get; set; }
}
