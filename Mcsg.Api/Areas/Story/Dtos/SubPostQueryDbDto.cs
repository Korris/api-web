namespace Mcsg.Api.Areas.Story.Dtos;

public class SubPostQueryDbDto : SubUploadFileDto
{
    public List<UploadFileQueryDbDto> FileDbs { get; set; }
}
