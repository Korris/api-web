using System.Text.Json.Serialization;

namespace Mcsg.Api.Areas.Social.Models;

using Common.Core.Enums;
using Common.SeedWork.Converters;
using Common.SeedWork.Enums;

public class MostReactionCommentResponse : BasicCommentResponse
{
    public float? Order { get; set; }
    public int ReplyCount { get; set; }
    public string? PostHashId { get; set; }
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
    public bool IsDeletedUser { get; set; }

    [JsonConverter(typeof(IsoDateTimeConverter))]
    public DateTime CreatedOn { get; set; }

    /// <summary>
    /// Due to changing the logic flow but not updating the UI code so ModifiedOn = CreatedOn
    /// </summary>
    [JsonConverter(typeof(IsoDateTimeConverter))]
    [Obsolete]
    public DateTime? ModifiedOn => CreatedOn;

    public string ResourceName { get; set; }
    public string ResourceUrl { get; set; }
    public MinioInstanceType? MinioInstance { get; set; }
    public string? BucketName { get; set; }
    public string ResourceHashId { get; set; }
    public string GifId { get; set; }
    public Guid PostId { get; set; }
    public List<UserMentionResponse> Mentions { get; set; } = new List<UserMentionResponse>();
    public string? CustomNote { get; set; }
    public ReplyResponse Replies { get; set; } = new ReplyResponse();
    public Guid? QuoteId { get; set; }
    public Guid? ParentId { get; set; }
    public ReactionsResponse Reaction { get; set; } = new ReactionsResponse();
}

public class CommentReactionResponseQuery
{
    public ReactionType? Type { get; set; }
    public Guid? TargetId { get; set; }
    public int Count { get; set; }
    public int ReactByCurrent { get; set; }
}
