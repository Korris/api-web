using System.Text.Json.Serialization;

namespace Mcsg.Api.Areas.Social.Models;

using Common.Core.Enums;
using Common.SeedWork.Converters;
using Common.SeedWork.Enums;

public class NewsFeedDto
{
    public string? UserAvatar { get; set; }
    public string? UserName { get; set; }
    public string? Body { get; set; }
    public string? HashId { get; set; }
    public string? HashPostId { get; set; }
    public string? ProfileName { get; set; }
    public bool IsSubPost { get; set; }

    [JsonConverter(typeof(IsoDateTimeConverter))]
    public DateTime CreatedOn { get; set; }

    public PostType Type { get; set; }
    public Guid Id { get; set; }
    public int? Order { get; set; }
    public string ResourceUrl { get; set; }
    public MinioInstanceType? MinioInstance { get; set; }
    public string? BucketName { get; set; }
    public string GifId { get; set; }
    public ReactionsResponse Reaction { get; set; }
}

