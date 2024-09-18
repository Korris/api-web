using System.ComponentModel.DataAnnotations;

namespace Mcsg.Common.Domain.Entities;

using Common.SeedWork.Enums;
using Core.Enums;
using SeedWork;
using SeedWork.Constants;

public partial class SystemResource : AuditableEntity
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
    public Guid? FeedbackId { get; set; }
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
}