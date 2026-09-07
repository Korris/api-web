namespace Mcsg.Api.Areas.Document.Models;

public class RelatedBoxResponse : RelatedBox
{
    public ReactionsResponse? Reaction { get; set; }
}

public class RelatedBoxQueryResponse : RelatedBox
{
    public string? ReactionStr { get; set; }
    public int TotalReacts { get; set; }

}

public class RelatedBox
{
    public string? ThumbnailUrl { get; set; }
    public string? Title { get; set; }
    public int TotalComment { get; set; }
    public string? HashId { get; set; }
    public string[] Tags { get; set; }
    public Guid Id { get; set; }
}
