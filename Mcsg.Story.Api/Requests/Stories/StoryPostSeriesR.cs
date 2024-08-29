namespace Mcsg.Story.Api.Requests;

using Common.Core.Enums;

public class StoryPostSeriesR
{
    public string? Title { get; set; }
    public string? Summary { get; set; }
    public string ThumbnailHashId { get; set; }
    public string CoverHashId { get; set; }
    public bool IsCurrentUserAuthor { get; set; }
    public string? AuthorName { get; set; } = string.Empty;
    public bool IsMature { get; set; } = false;
    public PostPermission Permission { get; set; }
    public List<string>? Tags { get; set; }
    public bool IsSaveAndPublish { get; set; }
}
