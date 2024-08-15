using System.Text.Json.Serialization;

namespace Mcsg.Social.Api.Models;

using Common.SeedWork.Converters;

public class ReportReferralResponse
{
    public Guid UserId { get; set; }

    [JsonConverter(typeof(IsoDateTimeConverter))]
    public DateTime CreatedOn { get; set; }

    public string? ProfileName { get; set; }
    public string? UserName { get; set; }
    public string? Avatar { get; set; }
}
