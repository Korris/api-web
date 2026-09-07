namespace Mcsg.Api.Areas.Comic.Dtos;

public class SubUploadFileDto : SubPostBasic
{
    public string ThumbnailUrl { get; set; }
    public List<UploadFileDto> Files { get; set; } = new List<UploadFileDto>();
    public string Body { get; set; }
}
