namespace Mcsg.Social.Api.Models;

using Common.Core.Enums;
using Enums;

public class PostSeriesResponse : PostResponse
{
    public int? ViewCount { get; set; }
    public int? CommentCount { get; set; }
    public int ChapterCount { get; set; }
    public string CoverUrl { get; set; }
    public bool IsMature { get; set; }
    public bool? IsCompleted { get; set; }
    public string ProfileName { get; set; }
    public string UserName { get; set; }
    public ChaptersExclusiveData FreeChapters { get; set; }
    public ChaptersExclusiveData ExclusiveChapters { get; set; }
    public ChaptersExclusiveData EstimateBuyChapters { get; set; }
    public ChaptersExclusiveData TotalChapters { get; set; }
    public List<ChapterBasicResponse> Chapters { get; set; } = new List<ChapterBasicResponse>();
    public string SeriesStatus { get; set; }
    public int TotalComment { get; set; }

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
    public int TotalReact { get; set; }
    public ReactionsResponse? Reaction { get; set; }
}
public class PostSeriesTopQueryDbResponse : PostSeriesResponse
{
    public Guid? AuthorId { get; set; }
    public string ProfileName { get; set; }
    public int TotalSubPostComment { get; set; }
    public string SubPostStr { get; set; }
    public PostSeriesSelectedType SelectType { get; set; }
    public string? ReactionByPostStr { get; set; }
    public int TotalReact { set; get; }
}

public class NewPostSeriesResponse : PostSeriesResponse
{
    public List<RewardRespone> Rewards { get; set; }
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
    public string[] Tags { get; set; }
    public string ThumbnailUrl { get; set; }
    public int CommentCount { get; set; }
    public int ReactionCount { get; set; }
    public bool IsMature { get; set; }
    public string HashId { get; set; }
    public PostType Type { get; set; }
    public List<ChapterBasicResponse> Chapters { get; set; } = new List<ChapterBasicResponse>();
}
