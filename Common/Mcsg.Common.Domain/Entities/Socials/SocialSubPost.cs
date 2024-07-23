using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

using Core.Enums;

[Table("SocialSubPosts")]
public class SocialSubPost : AuditableHasPrivateEntity
{
    public string? Title { get; set; }
    public string? Name { get; set; }
    public Guid PostId { get; set; }
    public string? HashId { get; set; }
    public Guid? AuthorId { get; set; }
    public Guid UserId { get; set; }
    public int Order { get; set; }
    public string? Body { get; set; }
    public string? CreatorNote { get; set; }
    public PostStatus Status { get; set; }
    public DateTime? PublishDate { get; set; }
    public int ViewCount { get; set; }
    public bool IsEnableComment { get; set; }
    public string? ExternalCode { get; set; }
    public bool IsExclusive { get; set; }
    public ComicExternalResource ExternalResource { get; set; }
}