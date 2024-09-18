namespace Mcsg.Common.Domain.Dtos;

using Common.Core.Enums;
using SeedWork.Enums;

public class FeedbackResourceDto
{
    public Guid Id { get; set; }
    public string HashId { get; set; }
    public string SubPostHashId { get; set; }
    public string Url { get; set; }
    public string Name { get; set; }
    public int Order { get; set; }
    public ResourceType Type { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public double Size { get; set; }
    public MinioInstanceType? MinioInstance { get; set; }
    public string? BucketName { get; set; }
}
