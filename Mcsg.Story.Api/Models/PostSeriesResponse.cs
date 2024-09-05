namespace Mcsg.Story.Api.Models;

using Common.Core.Enums;
using Dtos;
using Enums;

public class PostSeriesResponse : PostDto
{
    public int? ViewCount { get; set; }
    public int? CommentCount { get; set; }
    public int ChapterCount { get; set; }
    public string CoverUrl { get; set; }
    public string CoverHashId { get; set; }
    public string ThumbnailHashId { get; set; }
    public bool IsMature { get; set; }
    public bool? IsCompleted { get; set; }
    public string ProfileName { get; set; }
    public string? UserName { get; set; }
    public ChaptersExclusiveData FreeChapters { get; set; }
    public ChaptersExclusiveData ExclusiveChapters { get; set; }
    public ChaptersExclusiveData EstimateBuyChapters { get; set; }
    public ChaptersExclusiveData TotalChapters { get; set; }
    public List<ChapterBasicResponse> Chapters { get; set; } = new List<ChapterBasicResponse>();
    public string SeriesStatus { get; set; }
    public int TotalComment { get; set; }
    public bool IsFollowing { get; set; }
    public ReactionsResponse Reaction { get; set; }
    public DateTime LatestCreatedOn { get; set; }

    public HideOption Hide { get; set; }
    public bool HideIos => (Hide & HideOption.Ios) == HideOption.Ios;
    public bool HideAndroid => (Hide & HideOption.Android) == HideOption.Android;
    public bool HideWeb => (Hide & HideOption.Web) == HideOption.Web;
    public bool HideAll => (Hide & HideOption.All) == HideOption.All;
}
public class PostSeriesQueryDbResponse : PostSeriesResponse
{
    public Guid? AuthorId { get; set; }
}

public class PostSeriesAllTopResponse
{
    public List<PostSeriesTopResponse> TopHits { get; set; }
    public List<PostSeriesTopResponse> TopLatest { get; set; }
    public List<PostSeriesTopResponse> TopCompleted { get; set; }
}
public class PostSeriesTopResponse : PostSeriesResponse
{
    public Guid? AuthorId { get; set; }
    public string ProfileName { get; set; }
    public string UserName { get; set; }
    public int TotalReact { get; set; }
    public ReactionsResponse? Reaction { get; set; }
    public bool isNewChapter { get; set; }
    public int FollowCount { get; set; }
}
public class PostSeriesTopQueryDbResponse : PostSeriesResponse
{
    public Guid? AuthorId { get; set; }
    public string ProfileName { get; set; }
    public string UserName { get; set; }
    public int TotalSubPostComment { get; set; }
    public string SubPostStr { get; set; }
    public PostSeriesSelectedType SelectType { get; set; }
    public string? ReactionByPostStr { get; set; }
    public int TotalReact { set; get; }
}

public class NewPostSeriesResponse : PostSeriesResponse
{
    public List<RewardDto> Rewards { get; set; }
}
public class ChaptersExclusiveData
{
    public int Count { get; set; }
    public float Amount { get; set; }
}

public class PostBoxResposne
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }
    public string ProfileName { get; set; }
    public string UserName { get; set; }
    public string[] Tags { get; set; }
    public string ThumbnailUrl { get; set; }
    public int CommentCount { get; set; }
    public int ReactionCount { get; set; }
    public bool IsMature { get; set; }
    public string HashId { get; set; }
    public PostType Type { get; set; }
    public List<ChapterBasicResponse> Chapters { get; set; } = new List<ChapterBasicResponse>();

    public HideOption Hide { get; set; }
    public bool HideIos => (Hide & HideOption.Ios) == HideOption.Ios;
    public bool HideAndroid => (Hide & HideOption.Android) == HideOption.Android;
    public bool HideWeb => (Hide & HideOption.Web) == HideOption.Web;
    public bool HideAll => (Hide & HideOption.All) == HideOption.All;
}
