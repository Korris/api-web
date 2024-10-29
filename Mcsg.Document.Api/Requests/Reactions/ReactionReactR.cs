namespace Mcsg.Document.Api.Requests;

using Common.Core.Enums;

public class ReactionReactR
{
    public Guid TargetId { get; set; }
    public ReactionType Type { get; set; }
    public bool? IsReply { get; set; }
}
