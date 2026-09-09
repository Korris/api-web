namespace Mcsg.Api.Areas.Game.Models;

using Common.Core.Enums;

/// <summary>
/// Reaction count per type
/// </summary>
public class ReactionCountResponse
{
    public ReactionType Type { get; set; }
    public int Count { get; set; }
}

/// <summary>
/// Same shape as the Social / Comic "reaction" object so the frontend can reuse its widgets
/// </summary>
public class ReactionSummaryResponse
{
    public Guid TargetId { get; set; }
    public int TotalReacts { get; set; }
    public ReactionType? CurrentUserReactType { get; set; }
    public List<ReactionCountResponse> Reactions { get; set; } = new();
    public ReactionType? MostReactionType { get; set; }
}
