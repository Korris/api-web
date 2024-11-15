using System.Text.Json.Serialization;

namespace Mcsg.Social.Api.Models;

using Common.SeedWork.Converters;

public class FavoritePostByUserResponse
{
    public Guid Id { get; set; }

    [JsonConverter(typeof(IsoDateTimeConverter))]
    public DateTime CreatedOn { get; set; }

    public string? Title { get; set; }
    public string? Body { get; set; }
    public string? ThumbnailUrl { get; set; }
    public string[]? Tags { get; set; }
    public int TotalSubPost { get; set; }
    public int TotalComment { get; set; }
    public int TotalFollow { get; set; }
}
