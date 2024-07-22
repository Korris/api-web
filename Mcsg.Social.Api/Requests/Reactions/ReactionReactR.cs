namespace Mcsg.Social.Api.Requests;

using Common.Core.Enums;

public class ReactionReactR
{
    public Guid TargetId { get; set; }
    public ReactionType Type { get; set; }
}
