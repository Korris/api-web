namespace Mcsg.Common.Domain.Entities;

using Core.Enums;
using SeedWork;

public class BasePostComment : AuditableEntity
{
    public string? Body { get; set; }
    public string? CustomNote { get; set; }

    public Guid? ParentId { get; set; }
    public Guid PostId { get; set; }
    public Guid AuthorId { get; set; }
    public int Order { get; set; }
    public CommentStatus Status { get; set; }
    public Guid? ResourceId { get; set; }
    public string? GifId { get; set; }
    public Guid? QuoteId { get; set; }
}