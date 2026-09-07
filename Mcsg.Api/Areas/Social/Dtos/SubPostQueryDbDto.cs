namespace Mcsg.Api.Areas.Social.Dtos;

public class SubPostQueryDbDto : SubUploadFileDto
{
    public List<UploadFileQueryDbDto> FileDbs { get; set; }
}
