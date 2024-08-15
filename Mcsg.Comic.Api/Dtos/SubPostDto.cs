using System.Text.Json.Serialization;

namespace Mcsg.Comic.Api.Dtos;

using Common.SeedWork.Converters;

public class SubPostDto
{
    public string? Title { get; set; }
    public float Order { get; set; }

    [JsonConverter(typeof(IsoDateTimeConverter))]
    public DateTime CreatedOn { get; set; }
}
