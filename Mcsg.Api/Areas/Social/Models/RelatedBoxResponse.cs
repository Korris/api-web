namespace Mcsg.Api.Areas.Social.Models;

using Common.Core.Enums;

public class RelatedBoxResponse : RelatedBox
{
    public ReactionsResponse? Reaction { get; set; }
}

public class RelatedBoxQueryResponse : RelatedBox
{
    public string? ReactionStr { get; set; }
    public int TotalReacts { get; set; }
    public string PostType { get; set; }
}

public class RelatedBox
{
    public string? ThumbnailUrl { get; set; }
    public string? Title { get; set; }
    public int TotalComment { get; set; }
    public int ChapterCount { get; set; }
    public string? HashId { get; set; }
    public string[] Tags { get; set; }
    public Guid Id { get; set; }
    public HideOption Hide { get; set; }
    public PostType PostType { get; set; }
}
