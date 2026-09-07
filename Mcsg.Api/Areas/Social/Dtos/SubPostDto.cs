using System.Text.Json.Serialization;

namespace Mcsg.Api.Areas.Social.Dtos;

using Common.SeedWork.Converters;

public class SubPostDto
{
    public string? Title { get; set; }
    public int Order { get; set; }

    [JsonConverter(typeof(IsoDateTimeConverter))]
    public DateTime CreatedOn { get; set; }
}
