namespace Mcsg.Common.Domain.Entities;

using Core.Enums;
using SeedWork;

public class BasePostLink : AuditableEntity
{
    public Guid PostId { get; set; }
    public string? HashId { get; set; }
    public string? Url { get; set; }
    public string? Description { get; set; }
    public PostLinkType Type { get; set; }
}
