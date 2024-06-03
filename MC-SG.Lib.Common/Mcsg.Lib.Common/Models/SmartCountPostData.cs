using Mcsg.Lib.Data.Enums;
using Mcsg.Lib.Model.Enums;

namespace Mcsg.Lib.Common.Models
{
    public class SmartCountEntityData
    {
        public Guid EntityId { get; set; }
        public EntityType EntityType { get; set; }
        public EntitySubType? SubType { get; set; }
        public ActionType ActionType { get; set; }
        public bool IsRemove { get; set; }
    }
}
