namespace Mcsg.Api.Areas.Story.Models;

using Mcsg.Api.Areas.Story.Dtos;

public class ChapterBasicResponse : SubPostBasic
{
    public float Order { get; set; }
    public int? ViewCount { get; set; }
    public int CommentCount { get; set; }
    public Guid? PostId { get; set; }
    public bool IsPublicNow => PublishDate == null ? false : PublishDate.Value < DateTime.UtcNow;
    public bool IsEnableComment { get; set; }
    public Guid? UserExclusiveId { get; set; }
    public bool IsPremium { get; set; }
    public float Sort { get; set; }
}
public class ChapterResponse : ChapterBasicResponse
{
    public List<UploadFileDto> Files { get; set; } = new List<UploadFileDto>();
    public string Body { get; set; }
    public string ProfileName { get; set; }
    public List<RewardDto> Rewards { get; set; }
}
public class ChapterTOCResponse
{
    public float Order { get; set; }
    public string? Title { get; set; }
    public Guid Id { get; set; }
    public bool IsPremium { get; set; }
}
public class ChapterTOCExtendResponse : ChapterTOCResponse
{
    public DateTime? PublishDate { get; set; }
}

public class ChapterList
{
    public float Sort { get; set; }
    public float Order { get; set; }
    public string? Title { get; set; }
}