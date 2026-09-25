using System.Text.Json.Serialization;

namespace Mcsg.Api.Areas.TapShow.Models;

using Common.SeedWork.Converters;
using Common.SeedWork.Enums;

/// <summary>
/// Comment preview in post lists. Same JSON shape as Story / Comic MostReactionCommentResponse so the client can reuse the comment UI.
/// </summary>
public class MostReactionCommentResponse : BasicCommentResponse
{
    public float? Order { get; set; }
    public int ReplyCount { get; set; }
    public string? PostHashId { get; set; }
    public List<BasicCommentResponse>? ReplyData { get; set; }
}

public class BasicCommentResponse
{
    public Guid AuthorId { get; set; }
    public Guid Id { get; set; }
    public string? Body { get; set; }
    public string? Title { get; set; }
    public string? UserAvatar { get; set; }
    public string? ProfileId { get; set; }
    public string? AuthorName { get; set; }
    public string? UserName { get; set; }
    public bool IsDeletedUser { get; set; }

    [JsonConverter(typeof(IsoDateTimeConverter))]
    public DateTime CreatedOn { get; set; }

    /// <summary>
    /// Kept for the shared UI: ModifiedOn = CreatedOn (same as Story)
    /// </summary>
    [JsonConverter(typeof(IsoDateTimeConverter))]
    public DateTime? ModifiedOn => CreatedOn;

    public string? ResourceName { get; set; }
    public string? ResourceUrl { get; set; }
    public MinioInstanceType? MinioInstance { get; set; }
    public string? BucketName { get; set; }
    public string? ResourceHashId { get; set; }
    public string? GifId { get; set; }
    public Guid PostId { get; set; }
    public List<UserMentionResponse> Mentions { get; set; } = new();
    public string? CustomNote { get; set; }
    public ReplyResponse Replies { get; set; } = new();
    public Guid? QuoteId { get; set; }
    public Guid? ParentId { get; set; }
    public ReactionSummaryResponse Reaction { get; set; } = new();
}

public class ReplyResponse
{
    public int TotalReply { get; set; }
    public List<ReplyData> Data { get; set; } = new();
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

    [JsonConverter(typeof(IsoDateTimeConverter))]
    public DateTime? ModifiedOn => CreatedOn;

    public string ResourceHashId { get; set; } = string.Empty;
    public string ResourceUrl { get; set; } = string.Empty;
    public string GifId { get; set; } = string.Empty;
    public Guid? QuoteId { get; set; }
    public Guid PostId { get; set; }
    public string? CustomNote { get; set; }
    public ReactionSummaryResponse? Reaction { get; set; }
}

public class UserMentionResponse
{
    public Guid UserId { get; set; }
    public string? ProfileName { get; set; }
    public string? UserName { get; set; }
    public string? ProfileId { get; set; }
    public int Length { get; set; }
    public int Offset { get; set; }
}
