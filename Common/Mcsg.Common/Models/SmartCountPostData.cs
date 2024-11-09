namespace Mcsg.Common.Models;

using Core.Enums;

public class SmartCountEntityData
{
    public Guid EntityId { get; set; }
    public EntityType EntityType { get; set; }
    public EntitySubType? SubType { get; set; }
    public ActionType ActionType { get; set; }
    public bool IsRemove { get; set; }
}
