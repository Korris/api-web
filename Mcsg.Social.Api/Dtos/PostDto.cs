using System.Text.Json.Serialization;

namespace Mcsg.Social.Api.Dtos;

using Common.Core.Enums;
using Common.SeedWork.Converters;

public class PostDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string HashId { get; set; }
    public Guid UserId { get; set; }
    public string ProfileId { get; set; }
    public string AuthorName { get; set; }
    public bool IsCurrentUserIsAuthor { get; set; }
    public string ThumbnailUrl { get; set; }
    public string UserAvatar { get; set; }

    [JsonConverter(typeof(IsoDateTimeConverter))]
    public DateTime CreatedOn { get; set; }

    public PostType Type { get; set; }
    public string Body { get; set; }
    public string[] Tags { get; set; }
    public PostStatus Status { get; set; }
    public PostPermission Permission { get; set; }
    public List<SubUploadFileDto> SubPosts { get; set; } = [];
    public string CustomNote { get; set; }
}
