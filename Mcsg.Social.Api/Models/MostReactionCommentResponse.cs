using System.Text.Json.Serialization;

namespace Mcsg.Social.Api.Models;

using Common.SeedWork.Converters;

public class MostReactionCommentResponse : BasicCommentResponse
{
    public int? Order { get; set; }
    public int ReplyCount { get; set; }
    public List<BasicCommentResponse> ReplyData { get; set; }
}

public class BasicCommentResponse
{
    public Guid AuthorId { get; set; }
    public Guid Id { get; set; }
    public string Body { get; set; }
    public string Title { get; set; }
    public string UserAvatar { get; set; }
    public string ProfileId { get; set; }
    public string AuthorName { get; set; }
    public string? UserName { get; set; }

    [JsonConverter(typeof(IsoDateTimeConverter))]
    public DateTime CreatedOn { get; set; }

    public string ResourceName { get; set; }
    public string ResourceUrl { get; set; }
    public string ResourceHashId { get; set; }
    public string GifId { get; set; }
    public Guid PostId { get; set; }
    public List<UserMentionResponse> Mentions { get; set; } = new List<UserMentionResponse>();
    public string? CustomNote { get; set; }
    public ReplyResponse Replies { get; set; } = new ReplyResponse();
    public Guid QuoteId { get; set; }
    public DateTime? ModifiedOn { get; set; }
    public Guid? ParentId { get; set; }
}
