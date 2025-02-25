namespace Mcsg.Social.Api.Dtos;

using Common.Core.Enums;
using Common.Domain.Entities;
using Models;

public class FeedDto : PostDto
{
    public FeedDto()
    {
        Type = PostType.Feed;
    }

    public string UserName { get; set; }
    public string FullName { get; set; }
    public MetaDataDto MetaData { get; set; }
    public PostLinkDto Link { get; set; }
    public int TotalResource { get; set; }
    public List<ResourceDto> Resources { get; set; } = [];
    public BackgroundMedia.SearchDto? BackgroundSound { get; set; }
    public bool IsFavorite { get; set; }
    public bool IsFollowing { get; set; }
    public ReactionsResponse Reaction { get; set; }
    public SharePostResponse? SharePost { get; set; }
}

public class SharePostResponse
{
    public string? ThumbnailUrl { get; set; }
    public MetaDataDto? MetaData { get; set; }
    public PostLinkDto? Link { get; set; }
    public string? HashId { get; set; }
    public string? UserName { get; set; }
    public string? FullName { get; set; }
    public DateTime CreatedOn { get; set; }
    public Guid Id { get; set; }
    public int TotalResources { get; set; }
    public List<ResourceDto> Resources { get; set; } = [];
    public string? Body { get; set; }
    public string? ProfileId { get; set; }
    public Guid UserId { get; set; }
    public string? UserAvatar { get; set; }
    public HideOption Hide { get; set; }
    public PostStatus Status { get; set; }
    public string? Title { get; set; }
    public PostType Type { get; set; }
}

public class SharePostInput
{
    public Guid Id { get; set; }
    public PostType Type { get; set; }
}
