using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

using Core.Enums;
using SeedWork.Constants;

public partial class BasePost : AuditableHasPrivateEntity
{
    [StringLength(Validator.Title.Max)]
    public string? Title { get; set; }

    [StringLength(Validator.Hashtag.Max)]
    public string? HashId { get; set; }

    [StringLength(Validator.Url.Max)]
    public string? ThumbnailUrl { get; set; }

    [StringLength(Validator.Url.Max)]
    public string? CoverUrl { get; set; }

    [StringLength(Validator.Name.Max)]
    public string? AuthorName { get; set; }

    [StringLength(Validator.Description.Max)]
    public string? StatusReason { get; set; }

    [StringLength(Validator.ExternalCode.Max)]
    public string? ExternalCode { get; set; }

    public string? Body { get; set; }

    public string? CustomNote { get; set; }

    public Guid? AuthorId { get; set; }
    public Guid UserId { get; set; }
    public PostType Type { get; set; }
    public PostStatus Status { get; set; }
    public bool? IsMature { get; set; }
    public bool? IsCompleted { get; set; }
    public int ViewCount { get; set; }
    public ExternalResource ExternalResource { get; set; }
    public HideOption Hide { get; set; }

    [StringLength(Validator.ShortBody.Max)]
    public string? ShortBody { get; set; }

    [StringLength(Validator.ShortCustomNote.Max)]
    public string? ShortCustomNote { get; set; }

    public Guid? SharePostId { get; set; }

    public SharePostType? SharePostType { get; set; }

    /// <summary>
    /// NumberOfViews
    /// </summary>
    [NotMapped]
    public int NumberOfViews { get; set; }
}