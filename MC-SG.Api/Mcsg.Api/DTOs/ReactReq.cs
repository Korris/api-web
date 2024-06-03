using Mcsg.Lib.Data.Enums;

namespace Mcsg.Api.DTOs
{
    public class ReactReq
    {
        public Guid TargetId { get; set; }
        public ReactionType Type { get; set; }
    }
}
