namespace Mcsg.Comic.Api.Models;

using Lib.Data.Enums;

public class ReactionResponse
{
    public ReactionType Type { get; set; }
    public int Count { get; set; }
}
public class ReactionResponseQuery : ReactionResponse
{
    public int ReactByCurrent { get; set; }
}
public class ReactionsResponse
{
    public Guid TargetId { get; set; }
    public int TotalReacts { get; set; }
    public ReactionType? CurrentUserReactType { get; set; }
    public List<ReactionResponse> Reactions { get; set; }
    public ReactionType? MostReactionType { get; set; }
}
