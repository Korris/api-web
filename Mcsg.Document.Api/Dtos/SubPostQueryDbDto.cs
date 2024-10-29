namespace Mcsg.Document.Api.Dtos;

public class SubPostQueryDbDto : SubUploadFileDto
{
    public List<UploadFileQueryDbDto> FileDbs { get; set; }
}
