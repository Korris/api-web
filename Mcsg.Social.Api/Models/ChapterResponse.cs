namespace Mcsg.Social.Api.Models;

using Dtos;

public class ChapterBasicResponse : SubPostBasic
{
    public int Order { get; set; }
    public int? ViewCount { get; set; }
    public int CommentCount { get; set; }
    public Guid? PostId { get; set; }
    public bool IsPublicNow { get; set; }
    public bool IsEnableComment { get; set; }
    public Guid? UserExclusiveId { get; set; }
}
public class ChapterResponse : ChapterBasicResponse
{
    public List<UploadFileDto> Files { get; set; } = new List<UploadFileDto>();
    public string Body { get; set; }
    public string ProfileName { get; set; }
}
public class ChapterTOCResponse
{
    public int Order { get; set; }
    public string Title { get; set; }
    public Guid Id { get; set; }
}
public class ChapterTOCExtendResponse : ChapterTOCResponse
{
    public DateTime? PublishDate { get; set; }
}
