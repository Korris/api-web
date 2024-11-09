using System.Text.Json.Serialization;

namespace Mcsg.Document.Api.Models;

using Common.Core.Enums;
using Common.Core.Extensions;
using Common.SeedWork.Converters;
using Dtos;

public class PostBoxResponse : PostBox
{
    public List<SubPostDto>? Chapters { get; set; }
    public List<string>? Tags { get; set; }
    public bool IsNewChapter => LatestCreatedOn.IsNewChapter();
    public bool IsExternalSource { get; set; }

    [JsonConverter(typeof(IsoDateTimeConverter))]
    public DateTime LatestCreatedOn { get; set; }
}
public class PostBoxQueryResponse : PostBox
{
    public string? SubPosts { get; set; }
    public string? Tags { get; set; }
    public DateTime LatestCreatedOn { get; set; }
    [JsonIgnore]
    public ExternalResource ExternalResource { get; set; }
    public bool IsExternalSource => ExternalResource != ExternalResource.None;
}

public class PostBox
{
    public string? HashId { get; set; }
    public Guid Id { get; set; }
    public string? ThumbnailUrl { get; set; }
    public string? Body { get; set; }
    public string? Title { get; set; }
    public Guid AuthorId { get; set; }
    public string? AuthorName { get; set; }
    public int ViewCount { get; set; }
    public bool IsMature { get; set; }
    public bool IsCurrentUserAuthor { get; set; }
    public int ChapterCount { get; set; }
    public PostType Type { get; set; }

    [JsonConverter(typeof(IsoDateTimeConverter))]
    public DateTime CreatedOn { get; set; }

    public string? ProfileName { get; set; }
    public Guid UserId { get; set; }
    public string? UserName { get; set; }
    public ReactionsResponse Reaction { get; set; }

    public HideOption Hide { get; set; }
    public bool HideIos => (Hide & HideOption.Ios) == HideOption.Ios;
    public bool HideAndroid => (Hide & HideOption.Android) == HideOption.Android;
    public bool HideWeb => (Hide & HideOption.Web) == HideOption.Web;
    public bool HideAll => (Hide & HideOption.All) == HideOption.All;
    public PostStatus Status { get; set; }
    public bool IsArchived => Status == PostStatus.Inactive;
    public bool IsCensored { get; set; }
    public bool IsBlur { get; set; }
}
