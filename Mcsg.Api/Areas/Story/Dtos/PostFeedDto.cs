using System.Text.Json.Serialization;

namespace Mcsg.Api.Areas.Story.Dtos;

using Common.Core.Enums;
using Common.Core.Extensions;
using Common.Domain.Dtos;
using Common.SeedWork.Converters;
using Common.SeedWork.Dtos;
using Mcsg.Api.Areas.Story.Models;

/// <summary>
/// Feed item returned by PATCH v1/LoadFeed (same contract as api-mobile StoryPost.SearchDto)
/// </summary>
public class PostFeedDto : IdDto
{
    #region -- Properties --

    public string? Title { get; set; }

    public string HashId { get; set; } = null!;

    public string? ThumbnailUrl { get; set; }

    public string? Body { get; set; }

    public PostStatus Status { get; set; }

    public PostPermission Permission { get; set; }

    public bool? IsMature { get; set; }

    public bool? IsCompleted { get; set; }

    public int ViewCount { get; set; }

    public int ChapterCount { get; set; }

    public int FollowCount { get; set; }

    public string? CustomNote { get; set; }

    [JsonConverter(typeof(IsoDateTimeConverter))]
    public DateTime CreatedOn { get; set; }

    /// <summary>
    /// Latest chapter publish date, falls back to CreatedOn
    /// </summary>
    [JsonConverter(typeof(IsoDateTimeConverter))]
    public DateTime SortCreatedOn { get; set; }

    public int? TotalComments { get; set; }

    public ReactionsResponse? Reaction { get; set; }

    public CommentLateDto? LatestComment { get; set; }

    public bool IsFollowing { get; set; }

    public bool IsCurrentUserAuthor { get; set; }

    public bool IsNewChapter => SortCreatedOn.IsNewChapter();

    public bool IsBlur { get; set; }

    public bool IsCensored { get; set; }

    public string? AuthorName { get; set; }

    #endregion
}
