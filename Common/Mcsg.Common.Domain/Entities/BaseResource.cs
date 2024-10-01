using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

using Core.Enums;
using SeedWork;
using SeedWork.Constants;
using SeedWork.Enums;

public class BaseResource : AuditableEntity
{
    [StringLength(Validator.Title.Max)]
    public string? Title { get; set; }

    [StringLength(Validator.Name.Max)]
    public string? Name { get; set; }

    [StringLength(Validator.Hashtag.Max)]
    public string? HashId { get; set; }

    /// <summary>
    /// This object name
    /// </summary>
    [StringLength(Validator.Url.Max)]
    public string? Url { get; set; }

    /// <summary>
    /// Bucket name
    /// </summary>
    [StringLength(32)]
    public string? BucketName { get; set; }

    /// <summary>
    /// Minio instance
    /// </summary>
    public MinioInstanceType? MinioInstance { get; set; }

    public Guid? AuthorId { get; set; }
    public Guid? SubPostId { get; set; }
    public int Order { get; set; }

    /// <summary>
    /// Size (byte)
    /// </summary>
    public double Size { get; set; }

    /// <summary>
    /// Size of the compressed file in bytes.
    /// </summary>
    public double CompressedSize { get; set; }

    public int Width { get; set; }
    public int Height { get; set; }
    public ResourceType Type { get; set; }
    public ResourceLocationType LocationType { get; set; }
    public ResourceStatus Status { get; set; } = ResourceStatus.Done;

    [StringLength(Validator.Url.Max)]
    public string? ExternalUrl { get; set; }
    public ExternalResource? ExternalResource { get; set; }

    /// <summary>
    /// MicroService
    /// </summary>
    [NotMapped]
    public string MicroService { get; set; } = Core.Enums.MicroService.Social.ToString();
}