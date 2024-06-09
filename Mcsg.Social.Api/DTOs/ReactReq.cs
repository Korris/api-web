using Mcsg.Lib.Data.Enums;

namespace Mcsg.Social.Api.DTOs
{
    public class ReactReq
    {
        public Guid TargetId { get; set; }
        public ReactionType Type { get; set; }
    }
}
