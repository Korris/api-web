using System.Text.Json.Serialization;

namespace Mcsg.Comic.Api.Models;

using Common.Core.Enums;
using Common.SeedWork.Converters;
using Dtos;

public class PostBoxResponse : PostBox
{
    public List<SubPostDto>? Chapters { get; set; }
    public List<string>? Tags { get; set; }
}
public class PostBoxQueryResponse : PostBox
{
    public string? SubPosts { get; set; }
    public string? Tags { get; set; }
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
}
