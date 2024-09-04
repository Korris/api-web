namespace Mcsg.Social.Api.Dtos;

using Common.Core.Enums;
using Common.SeedWork.Enums;

public class UploadFileDto
{
    public string HashId { get; set; }
    public string SubPostHashId { get; set; }
    public string Url { get; set; }
    public string Name { get; set; }
    public int Order { get; set; }
    public ResourceStatus Status { get; set; }
    public ResourceType Type { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public double Size { get; set; }
    public MinioInstanceType? MinioInstance { get; set; }
}
