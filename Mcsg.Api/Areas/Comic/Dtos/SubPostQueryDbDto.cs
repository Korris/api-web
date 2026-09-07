namespace Mcsg.Api.Areas.Comic.Dtos;

public class SubPostQueryDbDto : SubUploadFileDto
{
    public List<UploadFileQueryDbDto> FileDbs { get; set; }
}
