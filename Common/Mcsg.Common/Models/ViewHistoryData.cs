namespace Mcsg.Lib.Common.Models;

using Mcsg.Common.Core.Enums;

public class ViewHistoryData
{
    public Guid UserId { get; set; }
    public Guid EntityId { get; set; }
    public EntityType EntityType { get; set; }
    public string IdAddress { get; set; }
    public EntitySubType? SubType { get; set; }
}
