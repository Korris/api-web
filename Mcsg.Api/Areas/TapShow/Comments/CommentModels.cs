using System.Text.Json.Serialization;

namespace Mcsg.Api.Areas.TapShow.Comments;

using Common.Core.Enums;
using Common.SeedWork.Converters;
using Common.SeedWork.Enums;
using Common.SeedWork.Responses;

// Comment / reaction responses cloned 1:1 from Areas/Story/Models so TapShow returns exactly the Story / Comic JSON.

public class CommentPagedResults<T> : PagedResponse<T>
{
    public int TotalComments { get; set; }

    public CommentPagedResults(int totalItems, int pageNumber = 1, int pageSize = 10) : base(totalItems, pageNumber, pageSize)
    {
    }
}

public class CommentResponse
{
    public Guid Id { get; set; }
    public Guid PostId { get; set; }
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
    public DateTime? ModifiedOn => CreatedOn;

    public string ResourceHashId { get; set; } = string.Empty;
    public string ResourceUrl { get; set; } = string.Empty;
    public string GifId { get; set; } = string.Empty;
    public int ReplyCount { get; set; }
    public ReactionsResponse Reaction { get; set; } = new ReactionsResponse();
    public ReplyResponse Replies { get; set; } = new ReplyResponse();
    public List<UserMentionResponse> Mentions { get; set; } = new List<UserMentionResponse>();
    public string? CustomNote { get; set; }
}

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
    public DateTime? ModifiedOn => CreatedOn;

    public string ResourceHashId { get; set; } = string.Empty;
    public string ResourceUrl { get; set; } = string.Empty;
    public string GifId { get; set; } = string.Empty;
    public Guid? QuoteId { get; set; } = null;
    public Guid PostId { get; set; }
    public string? CustomNote { get; set; }
    public ReactionsResponse Reaction { get; set; }
}

public class UserMentionResponse
{
    public Guid UserId { get; set; }
    public string ProfileName { get; set; }
    public string? UserName { get; set; }
    public string ProfileId { get; set; }
    public int Length { get; set; }
    public int Offset { get; set; }
}

public class ReactionResponse
{
    public ReactionType Type { get; set; }
    public int Count { get; set; }
}

public class ReactionResponseQuery : ReactionResponse
{
    public int ReactByCurrent { get; set; }
}

public class ReactionsResponse
{
    public Guid TargetId { get; set; }
    public int TotalReacts { get; set; }
    public ReactionType? CurrentUserReactType { get; set; }
    public List<ReactionResponse> Reactions { get; set; }
    public ReactionType? MostReactionType { get; set; }
}

public class ReactionUpdateResponse
{
    public Guid TargetId { get; set; }
    public Guid ReactionId { get; set; }
    public string MicroService { get; set; }
    public Guid? SubPostId { get; set; }
    public bool IsDeleted { get; set; }
}

public class ReactionsUserModel
{
    public ReactionType Type { get; set; }
    public Guid AuthorId { get; set; }
    public string AuthorName { get; set; }
    public string AuthorAvatar { get; set; }
    public string? UserName { get; set; }
    public bool IsFollowing { get; set; }
    public bool IsDeletedUser { get; set; }
}

#region -- Query models (Dapper) --

public class CommentQueryModel
{
    public Guid Id { get; set; }
    public Guid ParentId { get; set; }
    public Guid PostId { get; set; }
    public string Body { get; set; } = string.Empty;
    public DateTime CreatedOn { get; set; }
    public Guid AuthorId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string UserAvatar { get; set; } = string.Empty;
    public Guid ResourceId { get; set; }
    public string ResourceHashId { get; set; } = string.Empty;
    public string ResourceName { get; set; } = string.Empty;
    public string ResourceUrl { get; set; } = string.Empty;
    public MinioInstanceType? MinioInstance { get; set; }
    public string? BucketName { get; set; }
    public string GifId { get; set; } = string.Empty;
    public int CommentLevel { get; set; } = 0;
    public Guid? QuoteId { get; set; }
    public string? CustomNote { get; set; }
    public int ReplyCount { get; set; } = 0;
}

public class CommentQueryResult
{
    public Guid Id { get; set; }
    public Guid PostId { get; set; }
    public Guid AuthorId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string UserAvatar { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public DateTime CreatedOn { get; set; }
    public string ResourceHashId { get; set; } = string.Empty;
    public string ResourceName { get; set; } = string.Empty;
    public string ResourceUrl { get; set; } = string.Empty;
    public MinioInstanceType? MinioInstance { get; set; }
    public string? BucketName { get; set; }
    public Guid ReplyId { get; set; }
    public string ReplyAuthorName { get; set; } = string.Empty;
    public string ReplyUserAvatar { get; set; } = string.Empty;
    public string ReplyBody { get; set; } = string.Empty;
    public DateTime ReplyLastCreatedDate { get; set; }
    public string ReplyResourceHashId { get; set; } = string.Empty;
    public string ReplyResourceName { get; set; } = string.Empty;
    public string ReplyResourceUrl { get; set; } = string.Empty;
    public Guid ReplyQuoteId { get; set; } = Guid.Empty;
    public string GifId { get; set; } = string.Empty;
    public string ReplyGifId { get; set; } = string.Empty;
    public int TotalRecord { get; set; } = 0;
}

public class UserMentionModel
{
    public Guid Id { get; set; }
    public Guid LocationId { get; set; }
    public MentionLocationType LocationType { get; set; }
    public Guid EntityId { get; set; }
    public EntityType EntityType { get; set; }
    public string ProfileName { get; set; }
    public string? UserName { get; set; }
    public int Length { get; set; }
    public int Offset { get; set; }
    public string Text { get; set; }
}

public class CommentReactionResponseQuery
{
    public ReactionType? Type { get; set; }
    public Guid? TargetId { get; set; }
    public int Count { get; set; }
    public int ReactByCurrent { get; set; }
}

#endregion
