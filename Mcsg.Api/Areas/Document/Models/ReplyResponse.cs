using System.Text.Json.Serialization;

namespace Mcsg.Api.Areas.Document.Models;

using Common.SeedWork.Converters;

public class ReplyResponse
{
    public int TotalReply { get; set; } = 0;
    public List<ReplyData> Data { get; set; } = new List<ReplyData>();
}
public class ReplyData
{
    public Guid Id { get; set; }
    public Guid ParentId { get; set; }
    public Guid AuthorId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string UserAvatar { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;

    [JsonConverter(typeof(IsoDateTimeConverter))]
    public DateTime CreatedOn { get; set; }

    /// <summary>
    /// Due to changing the logic flow but not updating the UI code so ModifiedOn = CreatedOn
    /// </summary>
    [JsonConverter(typeof(IsoDateTimeConverter))]
    [Obsolete]
    public DateTime? ModifiedOn => CreatedOn;

    public string ResourceHashId { get; set; } = string.Empty;
    public string ResourceUrl { get; set; } = string.Empty;
    public string GifId { get; set; } = string.Empty;
    public Guid? QuoteId { get; set; } = null;
    public Guid PostId { get; set; }
    public string? CustomNote { get; set; }
    public ReactionsResponse Reaction { get; set; } = new ReactionsResponse();

}
