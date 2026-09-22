namespace Mcsg.Api.Areas.TapShow.Requests;

using Common.Core.Enums;
using Common.Core.Requests;

/// <summary>
/// React on a TapShow post or a TapShow post comment (TargetId = post id / comment id)
/// </summary>
public class ReactionReactR : BaseR
{
    public Guid TargetId { get; set; }
    public ReactionType Type { get; set; }
}
