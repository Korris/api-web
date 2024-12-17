using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

using Core.Enums;
using SeedWork.Constants;

public class BaseSubPost : AuditableHasPrivateEntity
{
    [StringLength(Validator.Title.Max)]
    public string? Title { get; set; }

    [StringLength(Validator.Name.Max)]
    public string? Name { get; set; }

    [StringLength(Validator.Hashtag.Max)]
    public string? HashId { get; set; }

    [StringLength(Validator.Url.Max)]
    public string? ThumbnailUrl { get; set; }

    public string? Body { get; set; }
    public string? CreatorNote { get; set; }

    public Guid PostId { get; set; }
    public Guid? AuthorId { get; set; }
    public Guid UserId { get; set; }
    public PostStatus Status { get; set; }

    [Column(TypeName = "timestamp")]
    public DateTime? PublishDate { get; set; }

    public int ViewCount { get; set; }
    public bool IsEnableComment { get; set; }
    public string? ExternalCode { get; set; }
    public bool IsExclusive { get; set; }
    public ExternalResource ExternalResource { get; set; }
    public float Order { get; set; }

    /// <summary>
    /// PostHashId
    /// </summary>
    [NotMapped]
    public string? PostHashId { get; set; }
}