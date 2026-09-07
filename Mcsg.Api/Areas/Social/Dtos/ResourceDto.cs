namespace Mcsg.Api.Areas.Social.Dtos;

using Common.Core.Enums;
using Common.SeedWork.Enums;

public class ResourceDto
{
    public ResourceType Type { get; set; }
    public ResourceStatus Status { get; set; }
    public int Order { get; set; }
    public string Url { get; set; }
    public string Name { get; set; }
    public string HashId { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public string Body { get; set; }
    public string? SubPostHashId { get; set; }
    public MinioInstanceType? MinioInstance { get; set; }
    public string? BucketName { get; set; }
    public Guid? Id { get; set; }
    public Guid? SubPostId { get; set; }
}
