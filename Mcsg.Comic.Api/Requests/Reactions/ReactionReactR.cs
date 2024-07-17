namespace Mcsg.Comic.Api.Requests;

using Lib.Data.Enums;

public class ReactionReactR
{
    public Guid TargetId { get; set; }
    public ReactionType Type { get; set; }
}
