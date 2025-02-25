using System.Text.Json.Serialization;

namespace Mcsg.Social.Api.Models;

using Common.Core.Enums;
using Common.SeedWork.Converters;
using Dtos;

public class FeedBoxResponse : FeedBox
{
    public List<SubUploadFileDto>? SubPosts { get; set; } = new List<SubUploadFileDto>();
    public List<ResourceDto>? Resources { get; set; } = new List<ResourceDto>();
    public MetaDataDto? MetaData { get; set; }
    public PostLinkDto? Link { get; set; }
    public bool IsFavorite { get; set; }
    public ReactionsResponse Reaction { get; set; }
    public SharePostResponse? SharePost { get; set; }
}

public class PostLinkFeedBoxResponse
{
    public string? HashId { get; set; } = "";
    public string? Url { get; set; } = "";
    public PostLinkType Type { get; set; } = 0;
}

public class FeedBoxQueryResponse : FeedBox
{
    public string? SubPosts { get; set; }
    public string? Resources { get; set; }
    public string? MetaDatas { get; set; }
}

public class FeedBox
{
    public Guid Id { get; set; }

    [JsonConverter(typeof(IsoDateTimeConverter))]
    public DateTime CreatedOn { get; set; }

    public Guid UserId { get; set; }
    public string? UserAvatar { get; set; }
    public string? Body { get; set; }
    public string? HashId { get; set; }
    public string? FullName { get; set; }
    public string? UserName { get; set; }
    public string? ProfileId { get; set; }
    public string? ThumbnailUrl { get; set; }
    public int TotalResources { get; set; }
    public PostType Type { get; set; }
    public string? Link { get; set; }
    public string? CustomNote { get; set; }
    public bool IsCurrentUserAuthor { get; set; }

    public HideOption Hide { get; set; }
    public bool HideIos => (Hide & HideOption.Ios) == HideOption.Ios;
    public bool HideAndroid => (Hide & HideOption.Android) == HideOption.Android;
    public bool HideWeb => (Hide & HideOption.Web) == HideOption.Web;
    public bool HideAll => (Hide & HideOption.All) == HideOption.All;
    public PostStatus Status { get; set; }
    public bool IsArchived => Status == PostStatus.Inactive;
    public bool IsCensored { get; set; }
    public bool IsBlur { get; set; }
    public Guid? SharePostId { get; set; }
    public PostType? SharePostType { get; set; }
    public string? Title { get; set; }
}
