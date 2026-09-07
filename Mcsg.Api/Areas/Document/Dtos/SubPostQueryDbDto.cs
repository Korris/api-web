namespace Mcsg.Api.Areas.Document.Dtos;

public class SubPostQueryDbDto : SubUploadFileDto
{
    public List<UploadFileQueryDbDto> FileDbs { get; set; }
}
