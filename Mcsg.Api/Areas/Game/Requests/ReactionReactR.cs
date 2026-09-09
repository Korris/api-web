namespace Mcsg.Api.Areas.Game.Requests;

using Common.Core.Enums;
using Common.Core.Requests;

/// <summary>
/// React on a game post or a game post comment (TargetId = post id / comment id)
/// </summary>
public class ReactionReactR : BaseR
{
    public Guid TargetId { get; set; }
    public ReactionType Type { get; set; }
}
