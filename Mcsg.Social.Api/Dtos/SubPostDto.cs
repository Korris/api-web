using System.Text.Json.Serialization;

namespace Mcsg.Social.Api.Dtos;

using Common.SeedWork.Converters;

public class SubPostDto
{
    public string? Title { get; set; }
    public int Order { get; set; }

    [JsonConverter(typeof(IsoDateTimeConverter))]
    public DateTime CreatedOn { get; set; }
}
